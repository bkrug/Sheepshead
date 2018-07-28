export class Offsetter {
    private _linearDocumentOffset: number = 0;
    private _lazyMaxOffset: number | null;

    //
    //The linear offset is the offset that would be used if we scrolled by a consistent amount with each wheel event.
    //
    public calculateLinearOffset(deltaY: number) {
        var scrollAmount = (deltaY > 0 ? 1 : -1) * window.innerHeight / 3;
        var newOffset = this._linearDocumentOffset + scrollAmount;
        newOffset = Math.max(newOffset, 0);
        newOffset = Math.min(newOffset, this.maxOffset());
        this._linearDocumentOffset = newOffset;
        return newOffset;
    }

    private maxOffset(): number {
        if (this._lazyMaxOffset == null) {
            var body = document.body,
                html = document.documentElement;
            var documentHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
            this._lazyMaxOffset = documentHeight - window.innerHeight;
        }
        return this._lazyMaxOffset;
    }

    //
    //The eased offset causes the document to scroll very slowly when we are near the edge of a page of directions and quickly otherwise.
    //
    public calculateEasedOffset(linearOffset: number): number {
        //The normalizedOffset imagins each page of directions to be 1.0 units in height.
        var normalizedOffset = linearOffset / window.innerHeight;
        var directionPage = normalizedOffset - normalizedOffset % 1;
        //Re-calculate the distance past the top of a page of directions that we've scrolled.
        var easedFraction = this.easeInOutCubicFraction(normalizedOffset);
        //Create a new offset.
        return (directionPage + easedFraction) * window.innerHeight;
    }

    private easeInOutCubicFraction(x: number): number {
        x = x % 1;
        if (x <= 0.5)
            return Math.pow(x * 2, 3) / 2;
        else
            return Math.pow(((x - 1) * 2), 3) / 2 + 1;
    }
}