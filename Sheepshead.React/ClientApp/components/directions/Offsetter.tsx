export class Offsetter {
    //
    //The eased offset causes the document to scroll very slowly when we are near the edge of a page of directions and quickly otherwise.
    //
    public calculateEasedOffset(linearOffset: number): number {
        //The normalizedOffset imagins each page of directions to be 1.0 units in height.
        var normalizedOffset = linearOffset / window.innerHeight;
        var slideIndex = normalizedOffset - normalizedOffset % 1;
        //Re-calculate the distance past the top of a page of directions that we've scrolled.
        var easedFraction = this.easeInOutCubicFraction(normalizedOffset);
        //Create a new offset.
        return (slideIndex + easedFraction) * window.innerHeight;
    }

    private easeInOutCubicFraction(x: number): number {
        x = x % 1;
        if (x <= 0.5)
            return Math.pow(x * 2, 3) / 2;
        else
            return Math.pow(((x - 1) * 2), 3) / 2 + 1;
    }
}