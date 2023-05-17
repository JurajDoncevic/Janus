#!/bin/bash

# Get input string from command line
input=$1

# Remove any whitespace from the input string
input=$(echo $input | tr -d '[:space:]')

# Check if input string is empty
if [ -z "$input" ]; then
  output=""
else
  # Replace commas with quotes and commas to create a JSON array
  output=$(echo $input | awk -v RS=',' '{printf "\"%s\",", $1}' | sed 's/,$//')
fi

# Print the JSON array
echo $output
