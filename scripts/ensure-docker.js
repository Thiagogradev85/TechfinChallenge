// Verifica Docker e sobe containers se necessário.
// Funciona em PowerShell, CMD e Git Bash — não depende de bash/WSL.
const { spawnSync } = require('child_process');

function exec(cmd) {
  return spawnSync(cmd, { shell: true, encoding: 'utf8' });
}

function run(cmd) {
  spawnSync(cmd, { shell: true, stdio: 'inherit' });
}

// 1. Docker Desktop está rodando?
if (exec('docker info').status !== 0) {
  console.error('\n  Docker Desktop nao esta rodando.');
  console.error('  Abra o Docker Desktop, aguarde iniciar e rode npm run dev novamente.\n');
  process.exit(1);
}

// 2. Container principal já está no ar?
const result = exec('docker ps --filter "name=techfin-kafka" --filter "status=running" -q');
if (result.stdout && result.stdout.trim().length > 0) {
  console.log('  Containers ja estao rodando — pulando docker-compose up.');
} else {
  console.log('  Subindo containers Docker...');
  run('docker-compose up -d');
  console.log('  Containers prontos.');
}
