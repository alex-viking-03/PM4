import unittest

def delivery_cost(distance):
    if distance < 0:
        raise ValueError
    elif distance <= 5:
        return 500
    return 500 + 100 * (distance - 5)

class DeliveryCostTest(unittest.TestCase):
    def test_edge(self):
        self.assertEqual(500, delivery_cost(5))

    def test_more_then_edge(self):
        self.assertEqual(600, delivery_cost(6))

    def test_less_then_edge(self):
        self.assertEqual(500, delivery_cost(4))

    def test_negative(self):
        with self.assertRaises(ValueError):
            delivery_cost(-1)