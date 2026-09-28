def fine_before(days):
    if days < 0:
        return "Ошибка"
    elif days <= 3:
        return 0
    else:
        return (days - 3) * 100

def fine_after(days):
    if days <= 3:
        return 0

    return days * 100

def fine_after_mine(days):
    if days < 0:
        return "Ошибка"
    elif days <= 3:
        return 0

    return (days - 3) * 100

days = [-1, 0, 2, 3, 4, 6]

for day in days:
    print("Входные данные: ", day)
    print()
    print("До: ", fine_before(day))
    print("После: ", fine_after(day))
    print("Моё: ", fine_after_mine(day ))
    print()