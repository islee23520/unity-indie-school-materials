---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# Windows WSL AI 코딩 환경 설정 가이드

## WSL2에서 AI 코딩 툴 설치하기

Windows PC에서 Linux 기반 개발 환경을 만들고 Codex, Claude Code, OpenCode를 설치합니다.

---

## 전체 진행 순서

1. WSL2를 이해하고 Windows에 Ubuntu를 설치합니다.
2. Linux 기본 패키지와 Node.js를 준비합니다.
3. AI 코딩 CLI를 하나씩 설치합니다.
4. WSL에서 자주 만나는 문제를 점검합니다.

---

# Part 1: WSL2 설치 및 기본 설정

Windows 안에 개발용 Linux 환경을 준비합니다.

---

## 1. WSL2란?

WSL2는 Windows에서 Linux 커널을 실행할 수 있게 해주는 개발 환경입니다.

1. Windows 앱은 그대로 사용하면서 Ubuntu 터미널을 함께 사용할 수 있습니다.
2. 대부분의 AI 코딩 CLI와 Node.js, Rust, Git 도구가 Linux 기준으로 안정적으로 동작합니다.
3. 프로젝트 파일은 가능하면 Linux 홈 디렉터리(`~`) 아래에 두는 것이 빠릅니다.
4. Unity Editor는 Windows에서 실행하고, CLI 기반 자동화는 WSL에서 실행하는 구성이 실용적입니다.

---

## 2. 설치 전 확인

1. Windows 10 최신 버전 또는 Windows 11을 사용합니다.
2. 관리자 권한 PowerShell 또는 Windows Terminal을 엽니다.
3. BIOS/UEFI에서 Virtualization이 켜져 있는지 확인합니다.
4. 회사 또는 학교 네트워크라면 proxy 정책을 먼저 확인합니다.

---

## 3. Windows에서 WSL2 활성화

관리자 권한 PowerShell에서 WSL을 설치합니다.

```bash
wsl --install
```

설치가 끝나면 Windows를 재부팅하고 Ubuntu 초기 계정을 생성합니다.

---

## 4. Ubuntu 24.04를 지정해서 설치

특정 Ubuntu 버전을 원하면 배포판 이름을 지정합니다.

```bash
wsl --install -d Ubuntu-24.04
```

이미 다른 Ubuntu가 설치되어 있다면 `wsl -l -v`로 목록을 확인한 뒤 필요한 배포판을 추가로 설치합니다.

---

## 5. 설치 상태 확인

WSL 배포판과 버전을 확인합니다.

```bash
wsl -l -v
```

`VERSION`이 `2`이면 WSL2로 실행 중입니다.

---

## 6. 기본 배포판 실행

Ubuntu 터미널을 실행하거나 Windows Terminal에서 Ubuntu 프로필을 엽니다.

```bash
wsl
```

처음 실행하면 Linux 사용자 이름과 비밀번호를 설정합니다.

---

## 7. 기본 패키지 업데이트

새 Ubuntu 환경에서는 먼저 패키지 목록과 설치된 패키지를 갱신합니다.

```bash
sudo apt update && sudo apt upgrade -y
```

이 단계는 이후 설치 오류를 줄이는 기본 준비입니다.

---

## 8. 개발 기본 패키지 설치

컴파일 도구, 다운로드 도구, Git, Node.js, npm을 설치합니다.

```bash
sudo apt install -y build-essential curl git nodejs npm
```

`build-essential`은 Rust crate나 네이티브 Node 패키지 빌드에도 자주 필요합니다.

---

## 9. Git 사용자 정보 설정

커밋을 만들 수 있도록 Git 사용자 정보를 설정합니다.

```bash
git config --global user.name "Your Name"
```

이름은 GitHub 계정 표시 이름과 맞추면 관리가 편합니다.

---

## 10. Node.js 최신 버전 준비: nvm 설치

Ubuntu 기본 Node.js는 버전이 낮을 수 있으므로 nvm을 사용합니다.

```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.0/install.sh | bash
source ~/.bashrc
nvm install --lts
nvm use --lts
nvm alias default lts/*
```

설치가 끝나면 `node --version`과 `npm --version`으로 확인합니다.

---

# Part 2: AI 코딩 툴 설치

WSL 터미널에서 사용할 AI 코딩 CLI를 설치합니다.

---

## 11. OpenAI Codex CLI 설치

npm 전역 패키지로 Codex CLI를 설치합니다.

```bash
npm install -g @openai/codex
```

설치 후 `codex --help`로 명령이 잡히는지 확인합니다.

---

## 12. Claude Code 설치

Claude Code 설치 스크립트를 실행합니다.

```bash
curl -fsSL https://claude.ai/install.sh | bash
```

설치 후 터미널을 새로 열어 `claude` 명령이 PATH에 잡히는지 확인합니다.

---

## 13. OpenCode 설치

OpenCode를 npm 전역 패키지로 설치합니다.

```bash
npm install -g opencode
```

설치 후 `opencode --help`로 설치를 확인합니다.

---

## 14. 설치 확인

각 CLI가 PATH에 잡혔는지 확인합니다.

```bash
codex --help
claude --help
opencode --help
```

세 명령 모두 정상적으로 도움말이 출력되면 설치가 완료된 것입니다.

---

# Part 3: 트러블슈팅

WSL에서 자주 발생하는 문제와 해결 방법입니다.

---

## 15. 명령을 찾지 못할 때: PATH 확인

설치했는데 `command not found`가 나오면 PATH가 제대로 설정되지 않은 것입니다.

```bash
which node
echo $PATH
```

`which` 명령으로 해당 실행 파일의 실제 위치를 찾고, 그 경로가 `$PATH`에 포함되어 있는지 확인합니다.

---

## 16. nvm과 apt Node.js 충돌 해결

`apt`로 설치한 Node.js와 `nvm`으로 설치한 Node.js가 충돌하면 `npm install -g`가 잘못된 위치에 설치됩니다.

**증상**: `codex --version`이 안 되거나, `npm -g list`에 패키지가 안 보임

**원인**: `which node`와 `which npm`이 서로 다른 경로를 가리킴

```bash
# 현재 어떤 node를 쓰고 있는지 확인
which node
# /usr/bin/node (apt) ← 문제

which npm
# /usr/bin/npm (apt) ← 문제
```

**해결 1단계**: apt Node.js를 제거합니다.

```bash
sudo apt remove -y nodejs npm
```

**해결 2단계**: nvm의 Node.js가 PATH 우선이 되도록 합니다.

```bash
source ~/.bashrc
nvm use --lts
which node
# /home/username/.nvm/versions/node/vXX.X.X/bin/node (nvm) ← 정상
```

**해결 3단계**: CLI를 다시 설치합니다.

```bash
npm install -g @openai/codex
codex --help
```

`which npm`이 nvm 경로를 가리키지 않으면 슬라이드 15의 PATH 설정을 다시 확인하세요.

---

## 17. nvm 설치 후 node를 찾지 못할 때

새 터미널에서 `node`가 안 잡히면 shell 설정 파일을 확인합니다.

```bash
# ~/.bashrc 끝에 nvm 초기화 코드가 있는지 확인
grep nvm ~/.bashrc
```

없으면 nvm 설치 스크립트가 정상 실행되지 않은 것입니다. 다시 설치합니다.

```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.0/install.sh | bash
source ~/.bashrc
```

zsh를 쓴다면 `~/.zshrc`를 확인합니다.

---

## 18. npm 전역 설치 위치: EACCES 에러

`npm install -g`에서 권한 에러가 나면 npm의 전역 설치 경로를 사용자 디렉터리로 바꿉니다.

```bash
mkdir -p ~/.npm-global
npm config set prefix '~/.npm-global'
```

그 다음 shell 설정 파일에 PATH를 추가합니다.

```bash
# ~/.bashrc 또는 ~/.zshrc에 추가
export PATH="$HOME/.npm-global/bin:$PATH"
source ~/.bashrc
```

이후 `npm install -g` 명령이 `sudo` 없이 동작합니다.

---

## 19. sudo 없이 Docker를 쓰고 싶을 때: usermod

Docker를 설치했는데 매번 `sudo docker`를 써야 한다면 사용자를 `docker` 그룹에 추가합니다.

```bash
sudo usermod -aG docker $USER
```

`-aG`는 기존 그룹을 유지하면서(`-a`) 새 그룹에 추가(`-G`)하는 옵션입니다. `usermod -G docker $USER`만 쓰면 다른 그룹에서 빠지게 되니 반드시 `-a`를 함께 씁니다.

적용하려면 WSL을 재시작합니다.

```bash
# WSL 종료 후 다시 실행
wsl --shutdown
wsl
```

재시작 후 `docker ps`가 `sudo` 없이 동작하면 성공입니다.

---

## 20. 그룹 권한 확인: groups 명령

현재 사용자가 속한 그룹을 확인합니다.

```bash
groups
```

Docker, dialout, sudo 등 필요한 그룹이 포함되어 있는지 확인합니다. 누락된 그룹이 있으면 `usermod -aG`로 추가합니다.

```bash
# 시리얼 포트 접근이 필요할 때
sudo usermod -aG dialout $USER
```

---

## 21. 네트워크 이슈: DNS 확인

패키지 설치가 느리거나 실패하면 DNS 응답부터 확인합니다.

```bash
nslookup github.com
```

응답이 없으면 Windows 네트워크, VPN, 회사 proxy, WSL DNS 설정을 순서대로 확인합니다.

WSL의 DNS를 수동으로 설정할 수도 있습니다.

```bash
# /etc/wsl.conf에 DNS 설정 추가
sudo tee /etc/wsl.conf > /dev/null <<EOF
[network]
generateResolvConf = false
EOF

# DNS 서버 수동 지정
sudo tee /etc/resolv.conf > /dev/null <<EOF
nameserver 8.8.8.8
nameserver 1.1.1.1
EOF
```

설정 후 `wsl --shutdown`으로 WSL을 재시작합니다.

---

## 22. 네트워크 이슈: Proxy 설정

proxy가 필요한 환경에서는 shell 환경 변수를 설정합니다.

```bash
export HTTPS_PROXY="http://proxy.example.com:8080"
export HTTP_PROXY="http://proxy.example.com:8080"
```

npm에도 proxy를 설정해야 할 수 있습니다.

```bash
npm config set proxy http://proxy.example.com:8080
npm config set https-proxy http://proxy.example.com:8080
```

영구 설정이 필요하면 `~/.bashrc`에 `export` 줄을 추가합니다.

---

## 22. 권한 문제: 실행 권한 부여

다운로드한 스크립트나 로컬 CLI가 실행되지 않으면 실행 권한을 확인합니다.

```bash
chmod +x ./script.sh
```

`sudo`는 시스템 경로에 설치할 때만 사용하고, 프로젝트 파일 소유권을 망가뜨리지 않도록 주의합니다.

---

## 23. 권한 문제: 파일 소유권 복구

실수로 `sudo`로 파일을 만들었거나 root 소유 파일이 생겼다면 현재 사용자로 되돌립니다.

```bash
sudo chown -R $USER:$USER .
```

프로젝트 루트에서 실행하기 전에 현재 위치가 맞는지 `pwd`로 확인합니다.

---

## 24. 권한 문제: sudo 없이 시스템 포트 사용

1024 이하 포트(80, 443)를 `sudo` 없이 쓰고 싶으면 리다이렉트로 우회합니다.

```bash
# 8080으로 서버를 띄우고 80 포트로 포워딩
sudo iptables -t nat -A PREROUTING -p tcp --dport 80 -j REDIRECT --to-port 8080
```

보통은 개발 서버를 3000, 8080 등 1024 이상 포트에서 띄우는 것으로 충분합니다.

---

## 25. WSL 재시작으로 설정 적용

시스템 설정 변경 후 WSL을 재시작합니다.

```bash
# Windows PowerShell에서
wsl --shutdown
wsl
```

`wsl --shutdown`은 모든 WSL 인스턴스를 종료합니다. 열려 있는 터미널이 있으면 먼저 저장하세요.

네트워크, 그룹 권한, `/etc/wsl.conf` 변경은 재시작 후에 적용됩니다.

---

## 26. 파일 시스템 성능: 작업 위치 선택

WSL에서 Node.js와 Git 작업은 Linux 홈 디렉터리 아래가 빠릅니다.

```bash
mkdir -p ~/workspace
```

`/mnt/c` 아래 Windows 파일 시스템에서 대량 파일 작업을 하면 npm install, git status, 빌드가 느려질 수 있습니다.

---

## 27. Windows PATH가 WSL에 유출되는 문제

WSL 터미널에서 `which node`가 Windows 쪽 경로를 가리키면 Windows PATH가 WSL로 유출된 것입니다.

```bash
# /etc/wsl.conf에 PATH 유출 방지 설정
sudo tee /etc/wsl.conf > /dev/null <<EOF
[interop]
appendWindowsPath = false
EOF
```

이후 `wsl --shutdown` 후 재시작하면 WSL이 순수 Linux PATH만 사용합니다. 단, Windows 실행 파일(`code.exe`, `explorer.exe` 등)을 WSL에서 직접 호출하려면 유출을 켜두어야 합니다.

---

## 28. 터미널 환경 변수 테스트

설정이 제대로 적용되었는지 터미널에서 확인합니다.

```bash
# Node.js 경로 확인
which node && node --version

# npm 전역 패키지 경로 확인
npm root -g

# shell 설정 파일에 PATH가 있는지 확인
grep -n "PATH" ~/.bashrc

# 현재 사용자와 그룹 확인
id
```

`id` 명령은 사용자 UID, 기본 GID, 속한 그룹을 모두 보여줍니다.

---

## 29. 최종 설치 확인 체크리스트

아래 항목을 순서대로 확인합니다.

1. `wsl -l -v`에서 Ubuntu가 VERSION 2로 표시됩니다.
2. `node --version`, `npm --version`이 정상 출력됩니다.
3. `git config --global --list`에 이름과 이메일이 있습니다.
4. `codex --help`, `claude --help`, `opencode --help`가 동작합니다.
5. `npm install -g`가 `sudo` 없이 동작합니다.
6. 프로젝트 파일은 WSL 홈 디렉터리 아래에 있습니다.

---

## 마무리

이제 Windows에서는 Unity와 브라우저를 사용하고, WSL2에서는 Git, Node.js, AI 코딩 CLI를 실행할 수 있습니다.

1. 새 프로젝트는 `~/workspace` 아래에 둡니다.
2. 각 AI CLI의 인증(API key, 브라우저 로그인 등)은 해당 도구의 공식 문서를 참고해 설정합니다.
3. 설치 명령이 실패하면 네트워크, PATH, 권한, 실행 위치를 순서대로 점검합니다.
