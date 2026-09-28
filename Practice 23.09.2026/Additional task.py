#Продемонстрирую работу своей функции на примере 3-его задания
def check_result(actual, expected):
    if actual == expected:
        return "Пройдено"

    return "Не пройдено"

def purchase_before(price, quantity):
    return price * quantity

def purchase_after(price, quantity):
    print(price * quantity)

print("Функция purchase_before")
print("Статус проверки:", check_result(1400, purchase_before(700, 2)))

print()

print("Функция purchase_after")
print("Статус проверки:", check_result(1400, purchase_after(700, 2)))