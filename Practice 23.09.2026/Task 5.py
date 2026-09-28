class StudentBefore:
    def __init__(self, name, score):
        self.name = name
        self.score = score
    def get_result(self):
        if self.score >= 50:
            return "Зачёт"

        return "Незачёт"

class GrantStudentBefore(StudentBefore):
    def get_result(self):
        if self.score >= 70:
            return "Грант сохранён"

        return "Грант не сохранён"


class StudentAfter:
    def __init__(self, name, score):
        self.name = name
        self.score = score

    def get_result(self):
        if self.score > 50:
            return "Зачёт"

        return "Незачёт"

class GrantStudentAfter(StudentAfter):
    pass

class MineStudent:
    def __init__(self, name, score):
        self.name = name
        self.score = score

    def get_result(self):
        if self.score >= 50:
            return "Зачёт"

        return "Не зачёт"

class MineGrantStudent(MineStudent):
    def get_result(self):
        if self.score >= 70:
            return "Грант сохранён"

        return "Грант не сохранён"


grades = [49, 50, 51]
grades_grant = [69, 70, 71]

print("Обычный студент")
for grade in grades:
    print("Входные данные", grade)
    Student1 = StudentBefore("Student before", grade)
    print("Имя:", Student1.name)
    print("До: ", Student1.get_result())

    Student2 = StudentAfter("Student after", grade)
    print("Имя:", Student2.name)
    print("После: ", Student2.get_result())

    StudentMine = MineStudent("Mine", grade)
    print("Имя:", StudentMine.name)
    print("До: ", StudentMine.get_result())

    print()

print()

print("Грантник")
for grade in grades_grant:
    print("Входные данные", grade)
    StudentGrant1 = GrantStudentBefore("Grant student before", grade)
    print("Имя:", StudentGrant1.name)
    print("До: ", StudentGrant1.get_result())

    StudentGrant2 = GrantStudentAfter("Grant student after", grade)
    print("Имя:", StudentGrant2.name)
    print("После: ", StudentGrant2.get_result())

    StudentGrantMine = MineGrantStudent("Mine grant student", grade)
    print("Имя:", StudentGrantMine.name)
    print("После: ", StudentGrantMine.get_result())

    print()

