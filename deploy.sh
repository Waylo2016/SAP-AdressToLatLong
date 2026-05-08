#!/usr/bin/env bash
set -euo pipefail

SKIP_SECRET_SETUP="${SKIP_SECRET_SETUP:-false}"

if [ "$SKIP_SECRET_SETUP" != "true" ]; then
  SKIP_SECRET_SETUP=true
  mkdir -p secrets

  if [ ! -f secrets/postgres_user.txt ]; then
    printf "geocoder_%s" "$(openssl rand -hex 6)" > secrets/postgres_user.txt
    echo "Generated secrets/postgres_user.txt"
  fi

  if [ ! -f secrets/postgres_password.txt ]; then
    openssl rand -base64 48 > secrets/postgres_password.txt
    echo "Generated secrets/postgres_password.txt"
  fi

  if [ ! -f secrets/sap_username.txt ]; then
    printf "CHANGE_ME" > secrets/sap_username.txt
    echo "Created secrets/sap_username.txt - please fill it in"
  fi

  if [ ! -f secrets/sap_password.txt ]; then
    printf "CHANGE_ME" > secrets/sap_password.txt
    echo "Created secrets/sap_password.txt - please fill it in"
  fi

  if [ ! -f secrets/geocoding_api.txt ]; then
    printf "CHANGE_ME" > secrets/geocoding_api.txt
    echo "Created secrets/geocoding_api.txt - please fill it in"
  fi

  chmod 600 \
    secrets/postgres_user.txt \
    secrets/postgres_password.txt \
    secrets/sap_username.txt \
    secrets/sap_password.txt \
    secrets/geocoding_api.txt

  for required_secret in \
    secrets/sap_username.txt \
    secrets/sap_password.txt \
    secrets/geocoding_api.txt
  do
    if [ "$(cat "$required_secret")" = "CHANGE_ME" ]; then
      echo "ERROR: $required_secret still contains CHANGE_ME. Please update it before deploying."
      exit 1
    fi
  done
else
  echo "Skipping secret setup because SKIP_SECRET_SETUP=true"
fi

: "${SAP_REST_CLIENT:?SAP_REST_CLIENT is required. Set it in your shell or .env file.}"

docker compose up -d --build