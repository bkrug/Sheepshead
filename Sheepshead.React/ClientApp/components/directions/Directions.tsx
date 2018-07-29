import '../../css/directions.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { Offsetter } from './Offsetter';

export interface DirectionsState {
}

export class Directions extends React.Component<RouteComponentProps<{}>, DirectionsState> {
    private _offsetter: Offsetter = new Offsetter();

    constructor(props: any) {
        super(props);
        this.state = {
            linearDocumentOffset: 0
        };
        this.onWheel = this.onWheel.bind(this);
    }

    //The linear offset is the offset that would be used if we scrolled by a consistent amount with each wheel event.
    //The eased offset causes the document to scroll very slowly when we are near the edge of a page of directions and quickly otherwise.
    private onWheel(e: any) {
        var newOffset = this._offsetter.calculateLinearOffset(e.deltaY);
        window.scroll(0, this._offsetter.calculateEasedOffset(newOffset));
    }

    public render() {
        return (
            <div className="directions-film" onWheel={this.onWheel}>
                <div className="slide">
                    <div className="content">
                        <h2>IDEA</h2>
                        <p>This version of sheepshead features 3 or 5 players. 1 "picker" against 2 other players, or 1 "picker" and (usually) 1 "partner" against 3 other players. Teams change each hand, and in the 5-player version it takes time to figure out who is on which team. </p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>GOAL</h3>
                        <p>The goal for each hand, is to win points for your team. One wins points, by winning tricks. (show two different tricks with different point values).</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>CARDS</h3>
                        <p>Sheepshead uses a 52-card deck, but throughs away the 2s, 3s, 4s, 5s, and 6s. There is a trump suit, and three fail suits (clubs, spades, and hearts).</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>SUITS</h3>
                        <p>The trump suit includes all Queens, Jacks, and Diamonds, omitting cards we threw away. The heart suit includes all hearts except Queens, Jacks, and cards we threw away. The other fail suits are just like hearts.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>TRICKS</h3>
                        <p>At the beginning of a trick, the starting player plays a card of a given suit. Each following player must play a card of the same suit, if he or she has it. Otherwise, following players can play any card they like.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>WINNING TRICKS</h3>
                        <p>The trick winner is the person who played the most powerful card. That total points in the trick depends on what cards were in the trick. (show two tricks, one with a trump winning, one with fail winning)</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>POWER AND POINTS</h3>
                        <p>This graph shows you relative power and exact points for each card. The most powerful card in a trick is the most powerful trump, if any trump was played. If all card played were fail cards, then the most power card from the suit that led determines the trick winner. This chart will be available as you play the game as a "cheat sheet".</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>DEAL</h3>
                        <p>In a 5-player game, each player is dealt 6 cards. In a 3-player game, each player is dealt 10 cards. The two remaining cards are the blinds. The picker gets the blinds.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>PICKING</h3>
                        <p>Each player gets a chance to decide to be or not to be a picker. Pickers are on the offense. Pickers get to take two extra blind cards and bury two cards. Pickers have the opportunity to earn more coins than the defensive players. Most pickers have several trump.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>PARTNERS</h3>
                        <p>There is more than one way to determine the partner. We'll teach you the Jack-of-Diamonds method first. The partner normally is the person with Jack-of-Diamonds in his or her hand. If the picker has Jack-of-Diamonds in his or her hand (including blinds), then the partner has the first card in this list not in the picker's hand.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>WINNING A HAND</h3>
                        <p>There are a total of 120 points in every hand. In order to win a hand, defenders must win at least 60 points. For offense to win a hand, they must have at least 61 points, including points in the buried cards.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>That's enough for now</h3>
                        <p>Go play a few hands, and come back to learn the game even better.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>COINS</h3>
                        <p>At the end of a hand each player wins or looses coins based on points they won. Coins accumulate over each hand, card points do not.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>COINS PER PLAYER</h3>
                        <p>In each hand, coins won by the offensive hand plus points by the defensive hand equals zero. This chart shows coins won or lost based on the points held by defensive side.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>LEASTERS</h3>
                        <p>When no one picks, a leasters round can start. In leasters, the player with the lowest number of points will win the round. The leasters winner must win at least one trick.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>LEASTERS WITH BLINDS</h3>
                        <p>In this edition of Sheepshead, blind cards are ignored in leasters. In many groups, house rules may assign the blinds the the player that wins the first leasters trick or the last leasters trick. In other groups, the dealer at the begining of the round, decides which trick will win the blinds along with the other cards in the trick.</p>
                    </div>
                </div>
                <div className="slide">
                    <div className="content">
                        <h3>CALLED-ACE</h3>
                        <p>A second method to identify a partner is the called-ace. The picker can choose the ace of a fail suit, and that card identifies the partner. The picker must have a card of the same suit in his or her hand. The picker must not lead a trick with his or her last card of the called suit. The partner must not lead with the Ace of the called suit.</p>
                    </div>
                </div>
            </div>
        );
    }
}