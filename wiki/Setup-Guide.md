# Setup Guide

## 요구 사항

| 구분 | 요구사항 |
|------|----------|
| Unity | 2020.3 LTS 이상 권장 |
| 플랫폼 | Windows / macOS |
| 지식 | Unity 2D 기본, C# 기본 |

## 프로젝트 열기

### 1. 리포지토리 클론

```bash
git clone https://github.com/minjungsung/shooting-game.git
cd shooting-game
```

### 2. Unity Hub에서 프로젝트 열기

1. Unity Hub 실행
2. **Projects** 탭 → **Open** 클릭
3. 클론한 `shooting-game` 디렉토리 선택
4. Unity 에디터 버전 선택 (2020.3 LTS 이상)
5. 프로젝트 열기

> **참고**: 이 리포는 스크립트 파일(.cs)만 포함하고 있습니다. Unity 프로젝트의 에셋, 씬 파일은 별도로 구성해야 합니다.

### 3. 프로젝트 구조 구성

스크립트 파일들을 Unity 프로젝트의 `Assets/Scripts/` 디렉토리에 배치합니다:

```
Assets/
├── Scripts/
│   ├── Player.cs
│   ├── Enemy.cs
│   ├── Bullet.cs
│   ├── Item.cs
│   ├── GameManager.cs
│   ├── ObjectManager.cs
│   ├── Follower.cs
│   ├── Explosion.cs
│   ├── Background.cs
│   └── Spawn.cs
├── Resources/
│   ├── Stage 0.txt        # 스테이지 0 스폰 데이터
│   ├── Stage 1.txt        # 스테이지 1 스폰 데이터
│   └── ...
├── Sprites/               # 스프라이트 에셋
├── Animations/             # 애니메이션 클립
├── Prefabs/                # 프리팹
│   ├── Player
│   ├── EnemyS / EnemyM / EnemyL / Boss
│   ├── BulletPlayerA / BulletPlayerB
│   ├── BulletEnemyA / BulletEnemyB
│   ├── BulletBossA / BulletBossB
│   ├── BulletFollower
│   ├── ItemCoin / ItemPower / ItemBoom
│   ├── Explosion
│   └── Follower
└── Scenes/
    └── MainScene.unity
```

### 4. 씬 설정

1. 새 씬 생성 또는 기존 씬 열기
2. GameManager 오브젝트 생성 → `GameManager.cs` 부착
3. ObjectManager 오브젝트 생성 → `ObjectManager.cs` 부착
4. Player 프리팹 생성 → `Player.cs` 부착
5. 적, 총알, 아이템 프리팹들 생성 및 ObjectManager에 연결

### 5. 스폰 데이터 작성

`Resources/Stage 0.txt` 예시:

```
2.0,EnemyS,0
0.5,EnemyS,2
1.0,EnemyM,1
3.0,EnemyL,3
5.0,Boss,1
```

### 6. 인스펙터 설정

**GameManager Inspector:**
- `player`: Player 게임오브젝트 연결
- `spawnPoints[]`: 적 스폰 위치 Transform 배열
- `objectManager`: ObjectManager 연결
- `scoreText`, `lifeImages[]`, `boomImages[]`: UI 요소 연결

**ObjectManager Inspector:**
- 각 프리팹 슬롯에 해당 프리팹 연결
- 모든 적, 총알, 아이템, 폭발 프리팹 필요

## 빌드

### PC 빌드

1. **File → Build Settings**
2. **Platform**: PC, Mac & Linux Standalone
3. **Add Open Scenes**: 현재 씬 추가
4. **Build** 클릭

### Android/iOS 빌드

1. **File → Build Settings**
2. **Platform**: Android 또는 iOS 선택
3. **Switch Platform**
4. **Player Settings**에서 해상도, 방향 설정
5. **Build** 클릭

## 입력 설정

기본 입력 매핑 (`Edit → Project Settings → Input Manager`):

| 입력 | 기본 키 | 용도 |
|------|---------|------|
| Horizontal | A/D, ←/→ | 좌우 이동 |
| Vertical | W/S, ↑/↓ | 상하 이동 |
| Fire1 | Left Ctrl, 마우스 왼쪽 | 발사 |
| Fire2 | Left Alt | 폭탄 |

모바일의 경우 `joyControl[]`과 `isButtonA/B` 변수로 가상 조이스틱 및 버튼 입력을 지원합니다.
