def print_cost_before(pages):
    total = pages * 30
    if pages >= 10:
        total = total * 0.9
    return total

def print_cost_after(pages):
    if pages >= 10:
        return pages * 30 - 0.1

    return pages * 30

def print_cost_mine(pages):
    if pages < 0:
        print("Количество страниц должно быть целым положительным числом")
        return None

    elif pages >= 10:
        return pages * 30 * 0.9

    return pages * 30

pages = [0, 1, 9, 10, 11, -1]

for page in pages:
    print("Входные данные: ", page)
    print()
    print("До: ", print_cost_before(page))
    print("После: ", print_cost_after(page))
    print("Моё: ", print_cost_mine(page))

    print()