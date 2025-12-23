using UnityEngine;
using System.Collections;

public class FlyingRangeBoss : EnemyAI
{
    [Header("--- CÀI ĐẶT DI CHUYỂN ---")]
    public float stopDistance = 5f;     // Khoảng cách dừng lại
    public float flyHeight = 2f;        // Bay cao hơn đầu Player bao nhiêu
    public float bobSpeed = 2f;         // Tốc độ nhấp nhô

    [Header("--- TẤN CÔNG (3 KHÚC) ---")]
    public Transform firePoint;         // Vị trí bắn (tay/miệng)
    public GameObject part1Prefab;      // Khúc 1 (Gốc)
    public GameObject part2Prefab;      // Khúc 2 (Giữa)
    public GameObject part3Prefab;      // Khúc 3 (Ngọn)
    public float segmentLength = 1.5f;  // Chiều dài mỗi khúc
    public float spawnDelay = 0.1f;     // Độ trễ giữa các khúc

    [Header("--- KỸ NĂNG: TELEPORT ---")]
    [Tooltip("Tỉ lệ dùng Teleport thay vì bắn thường (0-100)")]
    public int teleportChance = 30;
    [Tooltip("Thời gian tàng hình")]
    public float teleportDuration = 0.2f;
    [Tooltip("Khoảng cách xuất hiện sau lưng Player")]
    public float teleportOffset = 2.0f;

    [Header("--- KỸ NĂNG: RAGE MODE (HÓA ĐIÊN) ---")]
    public bool enableRage = true;
    public Color rageColor = Color.red; // Màu khi hóa điên
    public GameObject rageVFX;          // Hiệu ứng nổ khi gồng xong

    [Header("--- HIỆU ỨNG GỒNG (TRANSITION) ---")]
    public float chargeDuration = 3.0f; // Thời gian đứng gồng
    public GameObject rockPrefab;       // Prefab hòn đá rơi
    public float rockSpawnRate = 0.2f;  // Tốc độ rơi đá (giây/viên)

    // --- BIẾN NỘI BỘ ---
    private bool isRaging = false;
    private bool isCharging = false;    // Đang trong trạng thái gồng
    private SpriteRenderer mySprite;
    private Collider2D myCollider;
    private Vector3 originalScale;      // Lưu scale gốc

    protected override void Start()
    {
        base.Start();
        // Tắt trọng lực để bay
        rb.gravityScale = 0;
        attackRange = stopDistance;

        mySprite = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        originalScale = transform.localScale;
    }

    protected override void FixedUpdate()
    {
        if (isDead || playerTransform == null) return;

        // 1. NẾU ĐANG GỒNG -> ĐỨNG IM TUYỆT ĐỐI
        if (isCharging)
        {
            rb.velocity = Vector2.zero;
            animator.SetBool("isMoving", false);
            return; // Dừng mọi logic khác
        }

        // 2. LOGIC QUAY MẶT (Luôn nhìn về phía Player)
        // Dùng Mathf.Abs để tránh lỗi scale âm bị lật ngược hình
        float facingX = (playerTransform.position.x > transform.position.x) ? 1 : -1;
        float currentScaleX = Mathf.Abs(transform.localScale.x);
        transform.localScale = new Vector3(currentScaleX * facingX, transform.localScale.y, 1);

        // 3. DI CHUYỂN
        Vector2 targetPos = playerTransform.position;
        targetPos.y += flyHeight + Mathf.Sin(Time.time * bobSpeed) * 0.5f; // Cộng thêm nhấp nhô

        float distance = Vector2.Distance(transform.position, playerTransform.position);

        if (distance > stopDistance)
        {
            // Đuổi theo
            animator.SetBool("isMoving", true);
            rb.MovePosition(Vector2.MoveTowards(transform.position, targetPos, moveSpeed * Time.fixedDeltaTime));
        }
        else
        {
            // Dừng lại & Tấn công
            animator.SetBool("isMoving", false);
            rb.velocity = Vector2.zero;

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;

                // Random: Teleport hay Đánh?
                if (Random.Range(0, 100) < teleportChance)
                {
                    StartCoroutine(TeleportSkill());
                }
                else
                {
                    animator.SetTrigger("Attack");
                }
            }
        }
    }

    // =========================================================
    // PHẦN 1: LOGIC TELEPORT SÁT THỦ
    // =========================================================
    IEnumerator TeleportSkill()
    {
        // Dừng di chuyển
        rb.velocity = Vector2.zero;
        animator.SetBool("isMoving", false);

        // Tàng hình & Tắt va chạm
        if (mySprite) mySprite.enabled = false;
        if (myCollider) myCollider.enabled = false;

        yield return new WaitForSeconds(teleportDuration);

        // Tính toán vị trí SAU LƯNG Player
        // Lấy hướng nhìn của Player (-1 là trái, 1 là phải)
        float playerDir = Mathf.Sign(playerTransform.localScale.x);
        float behindDir = -playerDir;

        // Vị trí mới = Player + (Hướng sau lưng * Khoảng cách NGẮN)
        Vector2 teleportPos = new Vector2(
            playerTransform.position.x + (behindDir * teleportOffset),
            playerTransform.position.y + flyHeight
        );

        transform.position = teleportPos;

        // Hiện hình & Bật lại va chạm
        if (mySprite) mySprite.enabled = true;
        if (myCollider) myCollider.enabled = true;

        // Quay mặt về phía Player ngay lập tức
        float newFacing = (playerTransform.position.x > transform.position.x) ? 1 : -1;
        transform.localScale = new Vector3(Mathf.Abs(transform.localScale.x) * newFacing, transform.localScale.y, 1);

        // Đánh luôn cho bất ngờ (Delay cực ngắn 0.1s)
        yield return new WaitForSeconds(0.1f);
        animator.SetTrigger("Attack");
    }

    // =========================================================
    // PHẦN 2: LOGIC HÓA ĐIÊN (GỒNG -> RUNG -> ĐÁ RƠI -> BÙM)
    // =========================================================

    // Hàm này gọi từ script EnemyHealth khi máu < 50%
    public void ActivateRage()
    {
        if (isRaging) return; // Nếu đang Rage rồi thì thôi
        StartCoroutine(RageTransitionSequence());
    }

    IEnumerator RageTransitionSequence()
    {
        isRaging = true;
        isCharging = true; // Khóa di chuyển

        Debug.Log("BOSS BẮT ĐẦU GỒNG!");

        // 1. BẤT TỬ (Tắt Collider)
        if (myCollider) myCollider.enabled = false;

        // 2. RUNG MÀN HÌNH (Gọi Singleton CameraShake)
        if (CameraShake.Instance != null)
            CameraShake.Instance.Shake(chargeDuration, 0.3f);

        // 3. VÒNG LẶP GỒNG (3 Giây)
        float elapsed = 0f;
        float nextRockTime = 0f;
        Vector3 startPos = transform.position;

        while (elapsed < chargeDuration)
        {
            // A. Rung lắc bản thân con Boss tại chỗ
            transform.position = startPos + (Vector3)Random.insideUnitCircle * 0.1f;

            // B. Nhấp nháy màu cảnh báo (Đỏ/Trắng)
            if (mySprite) mySprite.color = Color.Lerp(Color.white, rageColor, Mathf.PingPong(Time.time * 20, 1));

            // C. Mưa đá rơi
            if (Time.time >= nextRockTime)
            {
                SpawnFallingRock();
                nextRockTime = Time.time + rockSpawnRate;
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        // 4. KẾT THÚC GỒNG
        transform.position = startPos; // Trả về vị trí cũ chuẩn
        if (myCollider) myCollider.enabled = true; // Hết bất tử
        isCharging = false; // Mở khóa di chuyển

        // 5. BUFF CHỈ SỐ
        moveSpeed *= 1.5f;       // Bay nhanh hơn
        attackCooldown *= 0.5f;  // Bắn nhanh gấp đôi
        spawnDelay /= 2f;        // Tốc độ chuỗi đạn nhanh hơn

        // Đổi màu đỏ vĩnh viễn
        if (mySprite) mySprite.color = rageColor;

        // Phóng to Boss (1.3 lần)
        transform.localScale = new Vector3(
            Mathf.Sign(transform.localScale.x) * Mathf.Abs(originalScale.x) * 1.3f,
            originalScale.y * 1.3f, 1);

        // Nổ VFX kết thúc
        if (rageVFX != null) Instantiate(rageVFX, transform.position, Quaternion.identity);

        Debug.Log("BOSS ĐÃ HÓA CHAOS XONG!");
    }

    // Hàm sinh đá rơi ngẫu nhiên
    void SpawnFallingRock()
    {
        // if (warningPrefab == null) return; // Nhớ đổi tên biến rockPrefab thành warningPrefab nhé

        // Lấy phạm vi camera
        float camHeight = Camera.main.orthographicSize;
        float camWidth = camHeight * Camera.main.aspect;
        Vector3 camPos = Camera.main.transform.position;

        // Random X trong màn hình
        float randomX = Random.Range(camPos.x - camWidth, camPos.x + camWidth);

        // QUAN TRỌNG: Y phải ở dưới đất (Gần chân Player)
        // Lấy Y của Player rồi trừ đi 1 xíu để nó nằm dưới chân
        float groundY = playerTransform.position.y - 1.5f;

        // Nếu game bro có sàn bằng phẳng cố định thì điền số cố định (ví dụ -3.5f)
        // Vector3 spawnPos = new Vector3(randomX, -3.5f, 0); 

        Vector3 spawnPos = new Vector3(randomX, groundY, 0);

        // Sinh ra CẢNH BÁO (chứ không phải sinh đá ngay)
        Instantiate(rockPrefab, spawnPos, Quaternion.identity);
        // (Lưu ý: Trong Inspector bro kéo Prefab CẢNH BÁO vào ô Rock Prefab nhé)
    }

    // =========================================================
    // PHẦN 3: HỆ THỐNG TẤN CÔNG (CHAIN ATTACK & SPREAD SHOT)
    // =========================================================

    // Hàm này được Animation Event gọi ("ShootProjectile")
    public void ShootProjectile()
    {
        // Khi Rage, tỉ lệ bắn chùm tăng từ 30% lên 60%
        int currentMultiChance = isRaging ? 60 : 30;

        if (Random.Range(0, 100) < currentMultiChance)
        {
            StartCoroutine(ShootSpread()); // Bắn chùm
        }
        else
        {
            StartCoroutine(SpawnChainAttack(0)); // Bắn thường
        }
    }

    // Bắn 1 chuỗi 3 khúc (Có thể chỉnh góc lệch)
    IEnumerator SpawnChainAttack(float angleOffset)
    {
        if (firePoint == null || playerTransform == null) yield break;

        // Tính góc quay về phía Player
        Vector2 direction = (playerTransform.position - firePoint.position).normalized;
        float baseAngle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;

        // Cộng thêm góc lệch (nếu bắn chùm)
        Quaternion rotation = Quaternion.AngleAxis(baseAngle + angleOffset, Vector3.forward);
        Vector3 finalDir = rotation * Vector3.right;

        // Khúc 1
        if (part1Prefab) Instantiate(part1Prefab, firePoint.position, rotation);
        yield return new WaitForSeconds(spawnDelay);

        // Khúc 2
        if (part2Prefab)
        {
            Vector3 pos2 = firePoint.position + (finalDir * segmentLength);
            Instantiate(part2Prefab, pos2, rotation);
        }
        yield return new WaitForSeconds(spawnDelay);

        // Khúc 3
        if (part3Prefab)
        {
            Vector3 pos3 = firePoint.position + (finalDir * segmentLength * 2);
            Instantiate(part3Prefab, pos3, rotation);
        }
    }

    // Bắn chùm (3 tia tỏa ra)
    IEnumerator ShootSpread()
    {
        StartCoroutine(SpawnChainAttack(0));   // Giữa
        StartCoroutine(SpawnChainAttack(20));  // Lệch lên 20 độ
        StartCoroutine(SpawnChainAttack(-20)); // Lệch xuống 20 độ
        yield return null;
    }
}