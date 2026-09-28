class Student:
    def __init__(self, name, score):
        self.name = name
        self.score = score

    def has_passed(self):
        return self.score > 50

    def add_points(self, points):
        self.score = self.score + points
        return self.score

class StudentCorrected:
    def __init__(self, name, score):
        self.name = name
        self.score = score

    def has_passed(self):
        return self.score >= 50

    def add_points(self, points):
        if points < 0:
            raise ValueError

        self.score = self.score + points

        if self.score > 100:
            self.score = 100

        return self.score
