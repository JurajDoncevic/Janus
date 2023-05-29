import requests
import csv

__subscriptions_dpc_path = "http://127.0.0.1:8803"
__customers_dpc_path = "http://127.0.0.1:8801"

__target_paths =[
    __subscriptions_dpc_path + "/Fresh/SingleUserSubscriptions",
    __subscriptions_dpc_path + "/Fresh/DualUserSubscriptions",
    __subscriptions_dpc_path + "/Fresh/MultiUserSubscriptions",
    __subscriptions_dpc_path + "/Stable/SingleUserSubscriptions",
    __subscriptions_dpc_path + "/Stable/DualUserSubscriptions",
    __subscriptions_dpc_path + "/Stable/MultiUserSubscriptions",
    __customers_dpc_path + "/Fresh/Customers",
    __customers_dpc_path + "/Stable/Customers"
]

__num_requests = 100
time_measurements: list[dict[str, float]] = [ dict() for i in range (0, __num_requests) ]
for target_path in __target_paths:
    for iteration in range(0, __num_requests):
        response = requests.get(target_path)
        total_seconds = response.elapsed.total_seconds()
        time_measurements[iteration][target_path] = total_seconds

with open("./time_measurements.csv", "w+") as file_output:
    dict_writer = csv.DictWriter(file_output, __target_paths, delimiter=';',lineterminator="\n")
    dict_writer.writeheader()
    for row in time_measurements:
        dict_writer.writerow(row)
