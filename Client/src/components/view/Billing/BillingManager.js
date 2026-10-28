// Rounds to paise. The server rounds the same way, so screen, print and saved bill agree.
export const round2 = (value) => Math.round((Number(value) + Number.EPSILON) * 100) / 100;

export const lineAmount = (item) =>
    round2((Number(item.mrp) || 0) * (Number(item.quantity) || 0) * (100 - (Number(item.discountPercentage) || 0)) / 100);

export const calculateTotalMrp = (billItems) =>
    round2(billItems.reduce((sum, x) => sum + (Number(x.mrp) || 0) * (Number(x.quantity) || 0), 0));

export const calculateNetAmount = (billItems) =>
    round2(billItems.reduce((sum, x) => sum + lineAmount(x), 0));

export const calculateTotalDiscount = (billItems) =>
    round2(calculateTotalMrp(billItems) - calculateNetAmount(billItems));

/**
 * CGST/SGST included in a saved bill, one entry per GST rate. Prices are MRPs, which include GST, and the
 * bill discount lowers each line in proportion. Lines without a rate (typed in by hand) carry no GST.
 */
export const gstBreakup = (bill) => {
    const subTotal = Number(bill.subTotal) || 0;
    const ratio = subTotal > 0 ? (Number(bill.totalAmount) || 0) / subTotal : 0;
    const byRate = new Map();
    (bill.billItems ?? []).forEach(item => {
        const rate = Number(item.tax) || 0;
        if (rate <= 0) return;
        const amount = (item.amount ?? lineAmount(item)) * ratio;
        byRate.set(rate, (byRate.get(rate) ?? 0) + amount);
    });
    return [...byRate.entries()]
        .sort(([a], [b]) => a - b)
        .map(([rate, amount]) => {
            const taxableValue = round2(amount * 100 / (100 + rate));
            const halfGst = round2((amount - amount * 100 / (100 + rate)) / 2);
            return { rate, taxableValue, cgst: halfGst, sgst: halfGst };
        });
};
