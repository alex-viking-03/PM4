import unittest
from course_service import Course
from course_service import IntensiveCourse

class TestCourse(unittest.TestCase):
    def test_new_course(self):
        course_name = "Проверка 1"
        course_capacity = 10

        course = Course(course_name, course_capacity)

        self.assertEqual(course.name, course_name)
        self.assertEqual(course.capacity, course_capacity)
        self.assertEqual(course.enrolled, 0)

    def test_no_name(self):
        course_name = ""
        course_capacity = 10

        with self.assertRaises(ValueError):
            course = Course(course_name, course_capacity)

    def test_zero_negative_capacity(self):
        course_name = "Проверка вместительности"
        course_negative_capacity = -10
        course_zero_capacity = 0

        with self.assertRaises(ValueError):
            course_negative = Course(course_name, course_negative_capacity)

        with self.assertRaises(ValueError):
            course_zero = Course(course_name, course_zero_capacity)

    def test_available_places(self):
        course = Course("Проверка доступные места", 10)
        self.assertEqual(course.available_places(), 10)

    def test_enroll(self):
        course = Course("Проверка занятия места", 10)

        course.enroll()
        course.enroll()
        course.enroll()

        self.assertEqual(course.enroll(), 6)

    def test_last_place(self):
        course = Course("Проверка последнее место курса", 1)

        course.enroll()

        self.assertEqual(course.enroll(), 0)

    def test_cancel_enrollment(self):
        course = Course("Проверка отмена занятия места", 10)

        course.enroll()
        course.enroll()

        course.cancel_enrollment()

        self.assertEqual(course.enrolled, 1)

    def test_cancel_zero_enroll(self):
        course = Course("Проверка отмены записи на нулевой курс", 10)

        with self.assertRaises(ValueError):
            course.cancel_enrollment()

    def test_is_full(self):
        course = Course("Проверка заполненности курса", 2)
        self.assertEqual(course.is_full, False)

        course.enroll()

        self.assertEqual(course.is_full, False)

        course.enroll()

        self.assertEqual(course.is_full, True)

    def test_intensive_course(self):
        with self.assertRaises(ValueError):
            intensive_course_more = IntensiveCourse("Проверка создание интенсивного курса 24", 10, 24)

        with self.assertRaises(ValueError):
            intensive_course_less = IntensiveCourse("Проверка создание интенсивного курса 5", 10, 5)

        intensive_course_edge_6 = IntensiveCourse("Проверка создание интенсивного курса 6", 10, 6)
        self.assertEqual(intensive_course_edge_6.hours_per_week, 6)

        intensive_course_10 = IntensiveCourse("Проверка создание интенсивного курса 10", 10, 10)
        self.assertEqual(intensive_course_10.hours_per_week, 10)

        intensive_course_11 = IntensiveCourse("Проверка создание интенсивного курса 11", 10, 11)
        self.assertEqual(intensive_course_11.hours_per_week, 11)

        intensive_course_20 = IntensiveCourse("Проверка создание интенсивного курса 20", 10, 20)
        self.assertEqual(intensive_course_20.hours_per_week, 20)

    def test_intensive_course_inheritance(self):
        intensive_course = IntensiveCourse("Проверка наследования", 10, 16)
        self.assertIsInstance(intensive_course, Course)

    def test_workload_level(self):
        intensive_course_high = IntensiveCourse("Проверка загруженности высокая", 10, 16)
        self.assertEqual(intensive_course_high.workload_level(), "высокая")

        intensive_course_average = IntensiveCourse("Проверка загруженности средняя", 10, 8)
        self.assertEqual(intensive_course_average.workload_level(), "средняя")

    def test_intensive_enroll(self):
        intensive_course = IntensiveCourse("Проверка бронирования места", 10, 10)
        self.assertEqual(intensive_course.enroll(), 9)

    def test_intensive_available_places(self):
        intensive_course = IntensiveCourse("Проверка свободных мест", 10, 10)
        self.assertEqual(intensive_course.available_places(), 10)

    def test_intensive_is_full(self):
        intensive_course = IntensiveCourse("Проверка курс заполнен", 1, 6)
        self.assertEqual(intensive_course.is_full, False)

        intensive_course.enroll()

        self.assertEqual(intensive_course.is_full, True)

    def test_intensive_from_full_to_not(self):
        intensive_course = IntensiveCourse("Проверка отмена записи и освобождение курса", 1, 6)

        intensive_course.enroll()

        self.assertEqual(intensive_course.is_full, True)

        intensive_course.cancel_enrollment()

        self.assertEqual(intensive_course.is_full, False)

    def test_waitlist(self):
        course = Course("Проверка списка ожидания", 1)
        course.enroll()

        course.enroll()
        self.assertEqual(course.waitlist, 1)