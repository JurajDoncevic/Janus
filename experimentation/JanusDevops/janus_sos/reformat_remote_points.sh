#!/bin/bash

# Check if argument was provided
if [ $# -eq 0 ]; then
  # echo "Usage: $0 <ipaddress:port,ipaddress:port...>"
  echo ""
  exit 1
fi

# Replace any spaces after the comma with an empty string
input_string=${1//, /,}

# Split the input string into an array
IFS="," read -r -a input_array <<< "$input_string"

# Create an empty array to hold the JSON objects
json_array=()

# Loop through the input array and create a JSON object for each IP address and port
for element in "${input_array[@]}"
do
  # Split the IP address and port into separate variables
  IFS=":" read -r ip_address port <<< "$element"

  # Create a JSON object for the IP address and port
  json_object="{\"Address\": \"$ip_address\", \"ListenPort\": $port}"

  # Add the JSON object to the array
  json_array+=("$json_object")
done

# Combine the JSON objects into a single array string with commas between them
json_string=$(printf '%s\n' "${json_array[@]}" | paste -sd ",")

# Output the JSON object with the "StartupRemotePoints" key
echo $json_string
