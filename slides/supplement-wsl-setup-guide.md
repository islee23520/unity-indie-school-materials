---
marp: true
theme: default
paginate: true
---

<!-- _class: lead -->

# Windows WSL AI 코딩 환경 설정 가이드

## WSL2에서 AI 코딩 툴과 하네싱 툴 준비하기

Windows PC에서 Linux 기반 개발 환경을 만들고 Codex, Claude Code, OpenCode, Gemini CLI, OMO, OMX, OMC, LUX를 설치합니다.

---

## 전체 진행 순서

1. WSL2를 이해하고 Windows에 Ubuntu를 설치합니다.
2. Linux 기본 패키지, Node.js, Rust/Cargo를 준비합니다.
3. AI 코딩 CLI를 하나씩 설치하고 인증합니다.
4. 하네싱 툴을 설치해 에이전트 워크플로우를 확장합니다.
5. WSL에서 자주 만나는 문제를 점검합니다.

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

## 10. Git 이메일 설정

GitHub에서 사용하는 이메일 또는 noreply 이메일을 설정합니다.

```bash
git config --global user.email "your-email@example.com"
```

설정 후 `git config --global --list`로 결과를 확인할 수 있습니다.

---

## 11. Node.js 최신 버전 준비: nvm 설치

Ubuntu 기본 Node.js는 버전이 낮을 수 있으므로 nvm을 사용합니다.

```bash
curl -o- https://raw.githubusercontent.com/nvm-sh/nvm/v0.40.0/install.sh | bash
```

설치 후 새 터미널을 열거나 shell 설정을 다시 불러옵니다.

---

## 12. nvm 설정 다시 불러오기

현재 터미널에서 바로 nvm을 사용하려면 `.bashrc`를 다시 로드합니다.

```bash
source ~/.bashrc
```

zsh를 사용한다면 `~/.zshrc`에 nvm 초기화 코드가 들어갔는지 확인합니다.

---

## 13. Node.js LTS 설치

AI CLI 대부분은 최신 LTS Node.js에서 안정적으로 동작합니다.

```bash
nvm install --lts
```

설치가 끝나면 `node --version`과 `npm --version`으로 확인합니다.

---

## 14. Node.js LTS 사용 설정

현재 터미널에서 LTS 버전을 사용하도록 전환합니다.

```bash
nvm use --lts
```

필요하면 `nvm alias default lts/*`로 기본 버전도 지정합니다.

---

## 15. Rust/Cargo 설치: LUX 준비

LUX Rust gateway CLI 빌드를 위해 Rust toolchain을 설치합니다.

```bash
curl --proto '=https' --tlsv1.2 -sSf https://sh.rustup.rs | sh
```

설치 중 기본 옵션을 선택하면 일반적인 개발 환경에 충분합니다.

---

## 16. Cargo 환경 변수 적용

현재 터미널에서 `cargo` 명령을 바로 사용하도록 환경을 불러옵니다.

```bash
source $HOME/.cargo/env
```

확인은 `rustc --version` 또는 `cargo --version`으로 진행합니다.

---

# Part 2: AI 코딩 툴 설치

WSL 터미널에서 사용할 AI 코딩 CLI를 설치합니다.

---

## 17. OpenAI Codex CLI 설치

npm 전역 패키지로 Codex CLI를 설치합니다.

```bash
npm install -g @openai/codex
```

설치 후 `codex --help`로 명령이 잡히는지 확인합니다.

---

## 18. OpenAI API Key 설정

현재 터미널 세션에서 API key를 환경 변수로 설정합니다.

```bash
export OPENAI_API_KEY="sk-..."
```

지속 설정이 필요하면 `~/.bashrc` 또는 `~/.zshrc`에 추가하되, 공유 저장소에는 절대 커밋하지 않습니다.

---

## 19. Claude Code 설치

Claude Code 설치 스크립트를 실행합니다.

```bash
curl -fsSL https://claude.ai/install.sh | bash
```

설치 후 터미널을 새로 열어 `claude` 명령이 PATH에 잡히는지 확인합니다.

---

## 20. Claude Code 인증

브라우저 인증 흐름을 통해 Claude 계정에 로그인합니다.

```bash
claude auth login
```

WSL에서 브라우저가 열리지 않으면 출력되는 URL을 Windows 브라우저에 복사해 진행합니다.

---

## 21. OpenCode 설치

OpenCode를 npm 전역 패키지로 설치합니다.

```bash
npm install -g opencode
```

프로젝트별 설정을 만들기 전 `opencode --help`로 설치를 확인합니다.

---

## 22. OpenCode 초기 설정

작업할 프로젝트 루트에서 OpenCode 설정을 초기화합니다.

```bash
opencode init
```

설정 파일에는 모델, provider, 로컬 규칙을 프로젝트 정책에 맞게 정리합니다.

---

## 23. Gemini CLI 설치 후보 1

환경에 따라 Anthropic namespace 패키지를 사용하는 안내가 있을 수 있습니다.

```bash
npm install -g @anthropic-ai/gemini-cli
```

패키지 이름은 배포 시점에 바뀔 수 있으므로 설치 실패 시 공식 문서를 확인합니다.

---

## 24. Gemini CLI 설치 후보 2

Google namespace 패키지를 사용하는 경우 다음 명령을 사용합니다.

```bash
npm install -g @google/gemini-cli
```

설치 후 `gemini --help` 또는 패키지가 안내하는 실행 명령으로 확인합니다.

---

# Part 3: 하네싱 툴 설치

여러 AI CLI 위에 워크플로우, 모드, 에이전트 구성을 얹는 도구를 설치합니다.

---

## 25. OMO 설치: oh-my-openagent

OpenCode 계열 환경을 확장하려면 OMO 설치 명령을 실행합니다.

```bash
bunx oh-my-opencode install
```

Bun이 없다면 먼저 Bun을 설치하거나 npm 기반 설치를 사용합니다.

---

## 26. OMO npm 설치 대안

전역 npm 패키지로 oh-my-openagent를 설치할 수 있습니다.

```bash
npm install -g oh-my-openagent
```

설치 후 프로젝트 정책에 맞게 agent, category, model routing 설정을 확인합니다.

---

## 27. OMX 설치: oh-my-codex

Codex와 OMX를 함께 전역 설치합니다.

```bash
npm install -g @openai/codex oh-my-codex
```

이미 Codex가 설치되어 있어도 같은 명령으로 버전을 맞출 수 있습니다.

---

## 28. OMX 실행 확인

고성능 모드 예시로 OMX를 실행합니다.

```bash
omx --madmax --high
```

처음 실행할 때는 API key와 프로젝트 권한 설정을 다시 확인합니다.

---

## 29. OMC 설치: Claude Code 플러그인 마켓

Claude Code 내부 명령에서 플러그인 마켓을 추가합니다.

```bash
/plugin marketplace add https://github.com/Yeachan-Heo/oh-my-claudecode
```

이 명령은 Claude Code 세션 안에서 실행하는 명령입니다.

---

## 30. OMC 플러그인 설치

추가한 마켓에서 oh-my-claudecode를 설치합니다.

```bash
/plugin install oh-my-claudecode
```

설치 후 Claude Code를 재시작하고 플러그인 명령이 보이는지 확인합니다.

---

## 31. OMC npm 설치 대안

Sisyphus 패키지를 npm 전역으로 설치하는 대안도 있습니다.

```bash
npm i -g oh-my-claude-sisyphus@latest
```

팀에서 사용하는 표준 방식이 있으면 한 가지 방식으로 통일합니다.

---

## 32. LUX 설치 위치 확인

LUX는 Unity 프로젝트의 패키지 내부 Rust gateway를 기준으로 설치합니다.

```bash
cargo install --path Packages/com.linalab.lux/RustGateway~ --force --locked
```

이 명령은 해당 `Packages/com.linalab.lux/RustGateway~` 경로가 있는 Unity 프로젝트 루트에서 실행합니다.

---

## 33. LUX 설치 확인

설치된 LUX CLI 버전을 확인합니다.

```bash
lux --version
```

명령을 찾지 못하면 `$HOME/.cargo/bin`이 PATH에 포함되어 있는지 확인합니다.

---

# Part 4: 트러블슈팅

WSL에서 자주 발생하는 문제를 빠르게 분리합니다.

---

## 34. WSL2 네트워크 이슈: DNS 확인

패키지 설치가 느리거나 실패하면 DNS 응답부터 확인합니다.

```bash
nslookup github.com
```

응답이 없으면 Windows 네트워크, VPN, 회사 proxy, WSL DNS 설정을 순서대로 확인합니다.

---

## 35. WSL2 네트워크 이슈: Proxy 설정

proxy가 필요한 환경에서는 shell 환경 변수를 설정합니다.

```bash
export HTTPS_PROXY="http://proxy.example.com:8080"
```

필요하면 `HTTP_PROXY`도 함께 설정하고, 인증 정보가 포함된 값은 공유하지 않습니다.

---

## 36. 권한 문제: 실행 권한 부여

다운로드한 스크립트나 로컬 CLI가 실행되지 않으면 실행 권한을 확인합니다.

```bash
chmod +x ./script.sh
```

`sudo`는 시스템 경로에 설치할 때만 사용하고, 프로젝트 파일 소유권을 망가뜨리지 않도록 주의합니다.

---

## 37. 권한 문제: 파일 소유권 복구

실수로 root 소유 파일이 생겼다면 현재 사용자로 되돌립니다.

```bash
sudo chown -R $USER:$USER .
```

프로젝트 루트에서 실행하기 전에 현재 위치가 맞는지 `pwd`로 확인합니다.

---

## 38. GUI 앱 접근: WSLg 확인

Windows 11의 WSLg는 Linux GUI 앱 실행을 지원합니다.

```bash
xeyes
```

`xeyes`가 없다면 `sudo apt install -y x11-apps`로 테스트 도구를 설치할 수 있습니다.

---

## 39. 파일 시스템 성능: 작업 위치 선택

WSL에서 Node.js와 Git 작업은 Linux 홈 디렉터리 아래가 빠릅니다.

```bash
mkdir -p ~/workspace
```

`/mnt/c` 아래 Windows 파일 시스템에서 대량 파일 작업을 하면 npm install, git status, 빌드가 느려질 수 있습니다.

---

## 40. Windows 파일 접근이 필요한 경우

Windows 사용자 폴더는 `/mnt/c/Users/...` 경로로 접근합니다.

```bash
ls /mnt/c/Users
```

대용량 프로젝트는 Windows 경로에서 직접 개발하지 말고 WSL 홈으로 복사해서 작업합니다.

---

## 41. PATH 설정: bash

bash 사용자는 `~/.bashrc`에 CLI 경로를 추가합니다.

```bash
export PATH="$HOME/.cargo/bin:$PATH"
```

설정 후 `source ~/.bashrc`를 실행하거나 터미널을 새로 엽니다.

---

## 42. PATH 설정: zsh

zsh 사용자는 `~/.zshrc`에 동일한 PATH 설정을 추가합니다.

```bash
export PATH="$HOME/.cargo/bin:$PATH"
```

shell 종류는 `echo $SHELL`로 확인합니다.

---

## 43. 최종 설치 확인 체크리스트

아래 항목을 순서대로 확인합니다.

1. `wsl -l -v`에서 Ubuntu가 VERSION 2로 표시됩니다.
2. `node --version`, `npm --version`, `cargo --version`이 정상 출력됩니다.
3. `git config --global --list`에 이름과 이메일이 있습니다.
4. AI CLI의 `--help` 명령이 동작합니다.
5. 프로젝트 파일은 WSL 홈 디렉터리 아래에 있습니다.

---

## 44. 최소 동작 확인 명령

마지막으로 주요 CLI 명령이 PATH에 잡혔는지 확인합니다.

```bash
codex --help
```

각 도구도 같은 방식으로 `claude --help`, `opencode --help`, `gemini --help`, `lux --version`을 확인합니다.

---

## 마무리

이제 Windows에서는 Unity와 브라우저를 사용하고, WSL2에서는 Git, Node.js, Rust, AI 코딩 CLI를 실행할 수 있습니다.

1. 새 프로젝트는 `~/workspace` 아래에 둡니다.
2. API key와 토큰은 shell 환경 변수나 안전한 secret store에 보관합니다.
3. 설치 명령이 실패하면 네트워크, PATH, 권한, 실행 위치를 순서대로 점검합니다.
