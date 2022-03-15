#!/bin/bash
ps aux | grep dotnet | kill -9 $(awk '{print $2}')
