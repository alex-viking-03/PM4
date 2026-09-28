#Первое задание
def print_cost(pages):
    if pages < 0:
        return 0

    total = pages * 30

    if pages > 10:
        total = total * 0.9

    return total

def print_cost_corrected(pages):
    if pages < 0:
        raise ValueError

    if pages >= 10:
        return pages * 30 * 0.9

    return pages * 30