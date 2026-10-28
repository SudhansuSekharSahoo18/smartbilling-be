import { calculateNetAmount, calculateTotalDiscount, calculateTotalMrp, gstBreakup, lineAmount, round2 } from './BillingManager';

test('line amounts are rounded to paise', () => {
  expect(lineAmount({ mrp: 99.99, quantity: 3, discountPercentage: 7 })).toBe(278.97);
  expect(round2(0.1 + 0.2)).toBe(0.3);
});

test('totals ignore blank rows and add up without floating-point noise', () => {
  const items = [
    { mrp: 1299, quantity: 1, discountPercentage: 7 },
    { mrp: null, quantity: 1, discountPercentage: 0 },
    { mrp: 499.5, quantity: 2, discountPercentage: 0 },
  ];
  expect(calculateTotalMrp(items)).toBe(2298);
  expect(calculateNetAmount(items)).toBe(2207.07);
  expect(calculateTotalDiscount(items)).toBe(90.93);
});

test('GST is worked out per rate from tax-inclusive amounts, after the bill discount', () => {
  const bill = {
    subTotal: 1640,
    totalAmount: 1476, // 10% bill discount
    billItems: [
      { amount: 1050, tax: 5 },
      { amount: 590, tax: 18 },
    ],
  };
  expect(gstBreakup(bill)).toEqual([
    // 1050 × 0.9 = 945 incl. 5% → taxable 900, GST 45
    { rate: 5, taxableValue: 900, cgst: 22.5, sgst: 22.5 },
    // 590 × 0.9 = 531 incl. 18% → taxable 450, GST 81
    { rate: 18, taxableValue: 450, cgst: 40.5, sgst: 40.5 },
  ]);
});

test('lines without a GST rate carry no GST', () => {
  const bill = { subTotal: 100, totalAmount: 100, billItems: [{ amount: 100, tax: null }, { amount: 0, tax: 0 }] };
  expect(gstBreakup(bill)).toEqual([]);
});
