import unittest
import ekeb_service_task_2


# Второе задание
class EkebTest2(unittest.TestCase):
    def test_exam_result(self):
        cases = [
            (0, "Незачёт"),
            (49, "Незачёт"),
            (50, "Зачёт"),
            (51, "Зачёт"),
            (100, "Зачёт")
        ]

        for result, expected in cases:
            self.assertEqual(ekeb_service_task_2.exam_result(result), expected)


    def test_exam_out_of_range(self):
        cases = [-1, 101]
        for case in cases:
            with self.assertRaises(ValueError):
                ekeb_service_task_2.exam_result(case)


    def test_exam_result_corrected(self):
        cases = [
            (0, "Незачёт"),
            (49, "Незачёт"),
            (50, "Зачёт"),
            (51, "Зачёт"),
            (100, "Зачёт")
        ]

        for result, expected in cases:
            self.assertEqual(ekeb_service_task_2.exam_result_corrected(result), expected)


    def test_exam_out_of_range_corrected(self):
        cases = [-1, 101]
        for case in cases:
            with self.assertRaises(ValueError):
                ekeb_service_task_2.exam_result_corrected(case)