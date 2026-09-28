#Второе задание
def exam_result(score):
    if score < 0 or score > 100:
        raise ValueError("Недопустимый балл")

    if score > 50:
        return "Зачёт"

    return "Незачёт"

def exam_result_corrected(score):
    if score < 0 or score > 100:
        raise ValueError("Недопустимый балл")

    if score >= 50:
        return "Зачёт"

    return "Незачёт"