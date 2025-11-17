#!/bin/bash

source .env
export PROJECT_DIR=$1
export OUT_DIR=$2

if [ -d "${PROJECT_DIR}${OUT_DIR}Mod" ]; then
  rm -rf ${PROJECT_DIR}${OUT_DIR}Mod
fi

mkdir ${PROJECT_DIR}${OUT_DIR}Mod

cp ${PROJECT_DIR}${OUT_DIR}CWMapApi.dll ${PROJECT_DIR}${OUT_DIR}Mod


cp ${PROJECT_DIR}include/* ${PROJECT_DIR}${OUT_DIR}Mod
if [ -d "${MODS_DIR}/CWMapApi" ]; then
  rm -rf ${MODS_DIR}/CWMapApi
fi
mv ${PROJECT_DIR}${OUT_DIR}Mod ${MODS_DIR}/CWMapApi