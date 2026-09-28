import unittest
from ekeb_service_task_4 import Student, GrantStudent

class TestStudent(unittest.TestCase):
    def setUp(self):
        self.grantStudent = GrantStudent("Грантник", 80)

    def test_student(self):
        self.assertIsInstance(self.grantStudent, Student)

    def test_status(self):
        cases = [
            (69, "Грант не сохранён"),
            (70, "Грант сохранён"),
            (71, "Грант сохранён")
        ]
        for case, expected in cases:
            self.grantStudent.score = case
            self.assertEqual(self.grantStudent.grant_status(), expected)

    def test_add_points(self):
        self.assertEqual(self.grantStudent.add_points(100), 100)

        self.test_status()