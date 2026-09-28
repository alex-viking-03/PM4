class Course:
    def __init__(self, name, capacity):
        if str(name) == "" or capacity <= 0:
            raise ValueError

        self.name = name
        self.capacity = capacity
        self.enrolled = 0
        self.is_full = False
        self.waitlist = 0

    def available_places(self):
        return self.capacity - self.enrolled

    def enroll(self):
        if self.is_full:
            self.waitlist += 1
            print(f"На курсе не осталось свободных мест. Студент добавлен в список ожидания. Ваша позиция: {self.waitlist}")
            return 0

        self.enrolled += 1

        if self.enrolled == self.capacity:
            if not self.is_full:
                self.is_full = True

            print(f"Занято последнее место. Регистрация следующих студентов невозможна")
            return 0

        print(f"Свободных мест на курсе: {self.capacity - self.enrolled}")
        return self.capacity - self.enrolled

    def cancel_enrollment(self):
        if self.enrolled == 0:
            raise ValueError

        if self.waitlist != 0:
            self.waitlist -= 1
            print(f"Выполнено перемещение из списка ожидания. Ожидающих: {self.waitlist}")
        self.enrolled -= 1


        if self.is_full:
            self.is_full = False

        print(f"Отмена записи успешна. Свободных мест: {self.capacity - self.enrolled}")

        return self.capacity - self.enrolled


class IntensiveCourse(Course):
    def __init__(self, name, capacity, hours_per_week):
        super().__init__(name, capacity)
        if hours_per_week <= 5 or hours_per_week >= 21:
            raise ValueError
        self.hours_per_week = hours_per_week

    def workload_level(self):
        if 6 <= self.hours_per_week <= 10:
            return "средняя"

        return "высокая"