# Game Objects

## 스크립트 상세 설명

### Player.cs — 플레이어

플레이어 캐릭터를 제어하는 핵심 스크립트입니다.

**상태 변수:**

| 변수 | 타입 | 설명 |
|------|------|------|
| `life` | int | 남은 생명 수 |
| `score` | int | 현재 점수 |
| `speed` | float | 이동 속도 |
| `power` | int | 공격력 레벨 (maxPower까지) |
| `boom` | int | 보유 폭탄 수 (maxBoom까지) |
| `isHit` | bool | 피격 상태 |
| `isBoomTime` | bool | 폭탄 사용 중 |
| `isRespawnTime` | bool | 무적 상태 |
| `isControl` | bool | 조작 가능 상태 |

**주요 동작:**

```
Update() 매 프레임:
  1. 이동 입력 처리 (키보드 / 조이스틱)
  2. 경계 충돌 감지 (isTouchTop/Bottom/Left/Right)
  3. 위치 업데이트 (speed * Time.deltaTime)
  4. 발사 로직 (Fire1 버튼 + 딜레이 체크)
  5. 폭탄 로직 (Fire2 버튼)

OnTriggerEnter2D():
  - Enemy/EnemyBullet 충돌 → 피격 처리
  - Item 충돌 → 파워/폭탄/코인 획득
```

**파워 레벨에 따른 발사 패턴:**

```
Power 1: BulletA × 1 (중앙)
Power 2: BulletA × 2 (좌우)
Power 3: BulletA × 2 + BulletB × 1
Power 4+: BulletA × 2 + BulletB × 2 + Follower 활성화
```

### Enemy.cs — 적

적 캐릭터의 AI, 체력, 공격 패턴을 관리합니다.

**적 타입별 초기화 (`OnEnable`):**

```csharp
switch (enemyName) {
    case "Boss": health = 100; Invoke("Stop", 2); break;
    case "S":    health = 3;   break;
    case "M":    health = 10;  break;
    case "L":    health = 40;  break;
}
```

**보스 전투:**

- `patternIndex`: 현재 패턴 번호
- `curPatternCount` / `maxPatternCount[]`: 패턴 반복 제어
- 패턴 완료 시 다음 패턴으로 전환
- Animator 기반 보스 전용 애니메이션

**피격 시 처리:**

```
OnHit(dmg):
  health -= dmg
  spriteRenderer → 피격 이펙트 (스프라이트 변경)
  
  if health <= 0:
    아이템 드롭 (랜덤: Coin, Power, Boom)
    Explosion 이펙트 생성
    gameManager.score += enemyScore
    SetActive(false) → 풀로 반환
```

### Bullet.cs — 총알

단순한 총알 오브젝트입니다.

| 변수 | 설명 |
|------|------|
| `dmg` | 대미지 값 |
| `isRotate` | 회전 여부 (특수 총알) |

```csharp
void Update() {
    if (isRotate) transform.Rotate(Vector3.forward * 10);
}

void OnTriggerEnter2D(Collider2D collision) {
    if (collision.gameObject.tag == "BorderBullet")
        gameObject.SetActive(false);  // 화면 밖 → 비활성화
}
```

### Item.cs — 아이템

적 처치 시 드롭되는 아이템입니다.

| 변수 | 설명 |
|------|------|
| `type` | 아이템 종류 ("Coin", "Power", "Boom") |

```csharp
void OnEnable() {
    rigid.velocity = Vector2.down * 1.5f;  // 아래로 천천히 이동
}
```

### Explosion.cs — 폭발 이펙트

Animator 기반의 폭발 시각 효과입니다.

```csharp
public void StartExplosion(string target) {
    anim.SetTrigger("OnExplosion");
    switch (target) {
        case "S":    localScale = 0.7f; break;
        case "M":
        case "P":    localScale = 1.0f; break;
        case "L":    localScale = 2.0f; break;
        case "Boss": localScale = 3.0f; break;
    }
}
```

적 크기에 따라 폭발 이펙트의 스케일이 달라집니다. 2초 후 자동 비활성화.

### Follower.cs — 보조 공격 유닛

플레이어를 따라다니며 자동으로 총알을 발사하는 보조 유닛입니다.

**추적 메커니즘:**

```
Queue<Vector3> parentPos → 부모(Player) 위치 이력 저장
followDelay → 큐에서 꺼낼 때까지의 지연 프레임 수

매 프레임:
  Watch(): 부모 위치를 큐에 삽입
  Follow(): 큐에서 위치를 꺼내 이동 (지연 추적 효과)
  Fire(): Fire1 입력 시 BulletFollower 발사
  Reload(): 연사 딜레이 관리
```

이 방식으로 플레이어 뒤를 일정 프레임 지연으로 따라다닙니다.

### GameManager.cs — 게임 관리자

게임의 전체 흐름을 제어합니다.

```
주요 역할:
1. 스테이지 시작/클리어/전환
2. 스폰 파일 읽기 (ReadSpawnFile)
3. 적 스폰 타이밍 관리
4. 점수/생명 UI 업데이트
5. 게임 오버 처리
6. 씬 재시작 (SceneManagement)
```

### ObjectManager.cs — 오브젝트 풀 관리자

모든 게임 오브젝트의 풀링을 담당합니다.

```
Awake():
  모든 타입의 오브젝트를 미리 Instantiate하여 풀에 저장
  SetActive(false) 상태로 대기

MakeObj(type):
  targetPool에서 비활성 오브젝트 검색
  있으면 → SetActive(true) 후 반환
  없으면 → Instantiate() 후 풀 추가 후 반환
```

### Background.cs — 배경 스크롤

```
Move(): Vector3.down * scrollSpeed * Time.deltaTime 으로 이동
Scrolling(): 맨 아래 스프라이트가 화면 밖이면 맨 위로 재배치
```

### Spawn.cs — 스폰 데이터

스폰 정보를 담는 순수 데이터 클래스입니다 (MonoBehaviour 미상속).

```csharp
public class Spawn {
    public float delay;   // 스폰 딜레이
    public string type;   // 적 타입
    public int point;     // 스폰 포인트 인덱스
}
```
