def result_before(score):
    if score >= 50:
        answer = "Зачёт"
    else:
        answer = "Незачёт"

    return answer

def result_after(score):
    if score > 50:
        return "Зачёт"

    return "Незачёт"

scores = [0, 49, 50, 51, 100]

for score in scores:
    print("Входные данные: ", score)
    print()
    print("До: ", result_before(score))
    print("После: ", result_after(score))
    print()