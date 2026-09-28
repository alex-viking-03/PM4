import unittest

from ekeb_service_task_3 import StudentCorrected

class ExcellentStudent(StudentCorrected):
    def scholarship(self):
        if 90 <= self.score <= 100:
            return 30000
        elif 75 <= self.score <= 89:
            return 15000
        elif self.score < 0:
            raise ValueError

        return 0

class TestExcellentStudent(unittest.TestCase):
    def setUp(self):
        self.excellent_student = ExcellentStudent("Excellent", 90)

    def test_scholarship(self):
        cases = [
            (74, 0),
            (75, 15000),
            (89, 15000),
            (90, 30000),
            (100, 30000)
        ]

        for case, expected in cases:
            self.excellent_student.score = case
            self.assertEqual(self.excellent_student.scholarship(), expected)

    def test_negative_scholarship(self):
        with self.assertRaises(ValueError):
            self.excellent_student.score = -1
            self.excellent_student.scholarship()