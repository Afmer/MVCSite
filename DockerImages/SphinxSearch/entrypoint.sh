#!/bin/bash

run_command() {
    indexer --all
    while [ $? -ne 0 ]; do
        sleep 10
        indexer --all
    done
}

run_command

searchd --nodetach &

cron

wait