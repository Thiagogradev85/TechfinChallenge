#!/usr/bin/env bash
# Garante que os containers Docker estejam rodando antes de subir a aplicação.
# Chamado pelo npm run dev — não precisa rodar diretamente.

# 1. Verifica se o daemon do Docker está acessível
if ! docker info >/dev/null 2>&1; then
  echo ""
  echo "  Docker Desktop não está rodando."
  echo "  Abra o Docker Desktop, aguarde iniciar e rode npm run dev novamente."
  echo ""
  exit 1
fi

# 2. Verifica se o container principal já está no ar
if docker ps --filter "name=techfin-kafka" --filter "status=running" -q | grep -q .; then
  echo "  Containers já estão rodando — pulando docker-compose up."
else
  echo "  Subindo containers Docker..."
  docker-compose up -d
  echo "  Containers prontos."
fi
