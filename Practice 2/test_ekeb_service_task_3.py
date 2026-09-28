from ekeb_service_task_3 import Student
from ekeb_service_task_3 import StudentCorrected

import unittest

class TestStudent(unittest.TestCase):
    def setUp(self):
        self.student = Student("Алия", 50)

    def test_name(self):
        self.assertEqual(self.student.name, "Алия")

    def test_score(self):
        self.assertEqual(self.student.score, 50)

    def test_has_passed(self):
        self.assertTrue(self.student.has_passed())

    def test_add_points(self):
        self.assertEqual(self.student.add_points(10), 60)

    def test_maximum(self):
        student = Student("Тест максимум", 90)
        self.assertEqual(student.add_points(20), 100)
        self.assertEqual(student.score, 100)

    def test_negative(self):
        student = Student("Тест отрицательное значение", 50)
        with self.assertRaises(ValueError):
            result = student.add_points(-20)

class TestStudentCorrected(unittest.TestCase):
    def setUp(self):
        self.student = StudentCorrected("Алия", 50)

    def test_name(self):
        self.assertEqual(self.student.name, "Алия")

    def test_score(self):
        self.assertEqual(self.student.score, 50)

    def test_has_passed(self):
        self.assertTrue(self.student.has_passed())

    def test_add_points(self):
        self.assertEqual(self.student.add_points(10), 60)

    def test_maximum(self):
        student = StudentCorrected("Тест максимум", 90)
        self.assertEqual(student.add_points(20), 100)
        self.assertEqual(student.score, 100)

    def test_negative(self):
        student = StudentCorrected("Тест отрицательное значение", 50)
        with self.assertRaises(ValueError):
            result = student.add_points(-20)