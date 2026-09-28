import unittest
import ekeb_service_task_1

class TestEkeb(unittest.TestCase):
    #Первое задание
    def test_print_cost(self):
        cases = [
            (0, 0),
            (1, 30),
            (9, 270),
            (10, 270),
            (11, 297)
        ]

        for pages, cost in cases:
            self.assertEqual(ekeb_service_task_1.print_cost(pages), cost)

    def test_negative_pages(self):
        with self.assertRaises(ValueError):
            ekeb_service_task_1.print_cost(-1)

    def test_print_cost_corrected(self):
        cases = [
            (0, 0),
            (1, 30),
            (9, 270),
            (10, 270),
            (11, 297)
        ]

        for pages, cost in cases:
            self.assertEqual(ekeb_service_task_1.print_cost_corrected(pages), cost)

    def test_negative_pages_corrected(self):
        with self.assertRaises(ValueError):
            ekeb_service_task_1.print_cost_corrected(-1)