import '../../css/directions.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface DirectionsState {
    linearDocumentOffset: number;
}

export class Directions extends React.Component<RouteComponentProps<{}>, DirectionsState> {
    constructor(props: any) {
        super(props);
        this.state = {
            linearDocumentOffset: 0
        };
        this.onWheel = this.onWheel.bind(this);
    }

    private lazyMaxOffset: number | null;
    private maxOffset(): number {
        if (this.lazyMaxOffset == null) {
            var body = document.body,
                html = document.documentElement;
            var documentHeight = Math.max(body.scrollHeight, body.offsetHeight, html.clientHeight, html.scrollHeight, html.offsetHeight);
            this.lazyMaxOffset = documentHeight - window.innerHeight;
        }
        return this.lazyMaxOffset;
    }

    //The linear offset is the offset that would be used if we scrolled by a consistent amount with each wheel event.
    //The eased offset causes the document to scroll very slowly when we are near the edge of a page of directions and quickly otherwise.
    private onWheel(e: any) {
        var newOffset = this.calculateLinearOffset(e);
        window.scroll(0, this.calculateEasedOffset(newOffset));
    }

    private calculateLinearOffset(e: any) {
        var scrollAmount = (e.deltaY > 0 ? 1 : -1) * window.innerHeight / 3;
        var newOffset = this.state.linearDocumentOffset + scrollAmount;
        newOffset = Math.max(newOffset, 0);
        newOffset = Math.min(newOffset, this.maxOffset());
        this.setState({ linearDocumentOffset: newOffset });
        return newOffset;
    }

    private calculateEasedOffset(linearOffset: number): number {
        //The normalizedOffset imagins each page of directions to be 1.0 units in height.
        var normalizedOffset = linearOffset / window.innerHeight;
        var directionPage = normalizedOffset - normalizedOffset % 1;
        //Re-calculate the distance past the top of a page of directions that we've scrolled.
        var easedFraction = this.easeInOutCubicFraction(normalizedOffset);
        //Create a new offset.
        return (directionPage + easedFraction) * window.innerHeight;
    }

    private easeInOutCubicFraction(x: number) : number {
        x = x % 1;
        if (x <= 0.5)
            return Math.pow(x * 2, 3) / 2;
        else
            return Math.pow(((x-1) * 2), 3) / 2 + 1;
    }

    public render() {
        return (
            <div className="directions-film" onWheel={this.onWheel}>
                <div className="page" style={{ backgroundColor: '#5ae' }}>
                    <div className="content">
                        <p>Sheepshead or Sheephead is a trick-taking card game related to the Skat family of games. It is the Americanized version of a card game that originated in Central Europe in the late 18th century under the German name Schafkopf. Sheepshead is most commonly played by five players,[1] but variants exist to allow for two to eight players. There are also many other variants to the game rules, and many slang terms used with the game.</p>
                        <p>Although Schafkopf literally means "sheepshead," it has nothing to do with sheep; the term probably was derived and translated incorrectly from Middle High German and referred to playing cards on a barrel head (from kopf, meaning head, and Schaff, meaning a barrel).[2]</p>
                        <p>In the United States, sheepshead is most commonly played in Wisconsin as well as the German counties in Southern Indiana, which has large German-American populations, and on the internet. Numerous tournaments are held throughout Wisconsin during the year, with the largest tournament being the "Nationals", held annually in the Wisconsin Dells during a weekend in September, October or November, and mini-tournaments held hourly throughout Germanfest in Milwaukee during the last weekend of each July.</p>
                    </div>
                </div>
                <div className="page" style={{ backgroundColor: '#ae5' }}>
                    <div className="content">
                        <h3>Preparation</h3>
                        <p>Sheepshead is played with 7-8-9-10-J-Q-K-A in four suits, for a total of 32 cards. This is also known as a Piquet deck, as opposed to the 52 or 54 present in a full French deck (also known as a Poker deck, or a regular deck of playing cards). A sheepshead deck is made by removing all of the jokers, sixes, fives, fours, threes, and twos from a standard deck. </p>
                    </div>
                </div>
                <div className="page" style={{ backgroundColor: '#e5a' }}>
                    <div className="content">
                        <h3>Card strength</h3>
                        <p>Card strength in sheepshead is different from in most other games. It is one of the most difficult things for some beginners to grasp.[1]</p>
                        <p>There are 14 cards in the trump suit: all four queens, all four jacks, and all of the diamonds. In order of strength from greatest to least:[1]</p>
                        <p>Q♣ Q♠ Q♥ Q♦</p>
                        <p>J♣ J♠ J♥ J♦</p>
                        <p>    A♦ 10♦ K♦ 9♦ 8♦ 7♦ </p>
                        <p>Also, there are 6 of each "fail" suit (18 total).[1]</p>
                        <p>    A, 10, K, 9, 8, and 7 of ♣, ♠, and ♥</p>
                        <p>Clubs, spades, and hearts take no precedence over other fail suits, unlike trump, which always take fail. (Notice how both aces and tens outrank kings; arguably the most confusing aspect of card strength). The lead suit must be followed if possible; if not, then any card may be played such as trump (which will take the trick), or a fail card. Playing a fail of a different suit is called "throwing off" and can be a way to clear up another suit. Additionally, throwing off a point card is called "schmearing." </p>
                    </div>
                </div>
                <div className="page" style={{ backgroundColor: '#ea5' }}>
                    <div className="content">
                        Page 4
                    </div>
                </div>
                <div className="page" style={{ backgroundColor: '#5ea' }}>
                    <div className="content">
                        Page 5
                    </div>
                </div>
                <div className="page" style={{ backgroundColor: '#a5e' }}>
                    <div className="content">
                        Page 6
                    </div>
                </div>
            </div>
        );
    }
}