#!/bin/bash

# Функция для выполнения команды и ожидания 10 секунд в случае ошибки
run_command() {
    indexer --all
    while [ $? -ne 0 ]; do
        sleep 10
        indexer --all
    done
}

run_command
searchd --nodetach