import '../../css/directions.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { CheatSheet } from '../game/CheatSheet';

export interface DirectionsState {
    filmOffset: string;
    currentSlide: number;
}

export class Directions extends React.Component<RouteComponentProps<{}>, DirectionsState> {
    private _AC: JSX.Element = (<img className='card' src={'./img/1.png'} alt='A♣' />);
    private _AS: JSX.Element = (<img className='card' src={'./img/2.png'} alt='A♠' />);
    private _AH: JSX.Element = (<img className='card' src={'./img/3.png'} alt='A♥' />);
    private _AD: JSX.Element = (<img className='card' src={'./img/4.png'} alt='A♦' />);
    private _KC: JSX.Element = (<img className='card' src={'./img/5.png'} alt='K♣' />);
    private _KS: JSX.Element = (<img className='card' src={'./img/6.png'} alt='K♠' />);
    private _KH: JSX.Element = (<img className='card' src={'./img/7.png'} alt='K♥' />);
    private _KD: JSX.Element = (<img className='card' src={'./img/8.png'} alt='K♦' />);
    private _QC: JSX.Element = (<img className='card' src={'./img/9.png'} alt='Q♣' />);
    private _QS: JSX.Element = (<img className='card' src={'./img/10.png'} alt='Q♠' />);
    private _QH: JSX.Element = (<img className='card' src={'./img/11.png'} alt='Q♥' />);
    private _QD: JSX.Element = (<img className='card' src={'./img/12.png'} alt='Q♦' />);
    private _JC: JSX.Element = (<img className='card' src={'./img/13.png'} alt='J♣' />);
    private _JS: JSX.Element = (<img className='card' src={'./img/14.png'} alt='J♠' />);
    private _JH: JSX.Element = (<img className='card' src={'./img/15.png'} alt='J♥' />);
    private _JD: JSX.Element = (<img className='card' src={'./img/16.png'} alt='J♦' />);
    private _10C: JSX.Element = (<img className='card' src={'./img/17.png'} alt='10♣' />);
    private _10S: JSX.Element = (<img className='card' src={'./img/18.png'} alt='10♠' />);
    private _10H: JSX.Element = (<img className='card' src={'./img/19.png'} alt='10♥' />);
    private _10D: JSX.Element = (<img className='card' src={'./img/20.png'} alt='10♦' />);
    private _9C: JSX.Element = (<img className='card' src={'./img/21.png'} alt='9♣' />);
    private _9S: JSX.Element = (<img className='card' src={'./img/22.png'} alt='9♠' />);
    private _9H: JSX.Element = (<img className='card' src={'./img/23.png'} alt='9♥' />);
    private _9D: JSX.Element = (<img className='card' src={'./img/24.png'} alt='9♦' />);
    private _8C: JSX.Element = (<img className='card' src={'./img/25.png'} alt='8♣' />);
    private _8S: JSX.Element = (<img className='card' src={'./img/26.png'} alt='8♠' />);
    private _8H: JSX.Element = (<img className='card' src={'./img/27.png'} alt='8♥' />);
    private _8D: JSX.Element = (<img className='card' src={'./img/28.png'} alt='8♦' />);
    private _7C: JSX.Element = (<img className='card' src={'./img/29.png'} alt='7♣' />);
    private _7S: JSX.Element = (<img className='card' src={'./img/30.png'} alt='7♠' />);
    private _7H: JSX.Element = (<img className='card' src={'./img/31.png'} alt='7♥' />);
    private _7D: JSX.Element = (<img className='card' src={'./img/32.png'} alt='7♦' />);
    private _6C: JSX.Element = (<img className='card' src={'./img/33.png'} alt='6♣' />);
    private _5S: JSX.Element = (<img className='card' src={'./img/38.png'} alt='5♠' />);
    private _4H: JSX.Element = (<img className='card' src={'./img/43.png'} alt='4♥' />);
    private _3D: JSX.Element = (<img className='card' src={'./img/48.png'} alt='3♦' />);
    private _2C: JSX.Element = (<img className='card' src={'./img/49.png'} alt='2♣' />);
    private _back: JSX.Element = (<img className='card' src={'./img/b1fv.png'} alt='card-back' />);

    private _inputNodes: { [slideName: string]: HTMLDivElement } = {};
    private _basicSlides: { [slideName: string]: JSX.Element } = {
        concept:
            (
                <div>
                    <h2>CONCEPT</h2>
                    <p>
                        This version of sheepshead features 3 or 5 players.
                        1 'picker' against 2 other players,
                        or 1 'picker' and (usually) 1 'partner' against 3 other players.
                        Teams change each hand, and in the 5-player version it takes time to figure out who the partner is.
                    </p>
                    <div className='scenarios'>
                        <div className='column' style={{ textAlign: 'right' }}>
                            <div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Picker</div>
                                </div>
                            </div>
                            <div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Picker</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Partner</div>
                                </div>
                            </div>
                            <div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Picker</div>
                                </div>
                            </div>
                        </div>
                        <div className='column' style={{ textAlign: 'left' }}>
                            <div>
                                <div className='vs'>
                                    vs.
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                            </div>
                            <div>
                                <div className='vs'>
                                    vs.
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                            </div>
                            <div>
                                <div className='vs'>
                                    vs.
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                                <div className='player'>
                                    <div>👤</div>
                                    <div>Defense</div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
            ),
        goal:
            (
                <div>
                    <h3>GOAL</h3>
                    <p>
                        The goal for each hand, is to win points for your team.
                        A player wins points for the team by winning tricks.
                    </p>
                    <div className='trickRow'>
                        <div className='cardCol'>
                            <div>{this._8S}</div>
                            <div>0</div>
                        </div>
                        <div className='cardCol'>
                            <div>{this._AS}</div>
                            <div>11</div>
                        </div>
                        <div className='cardCol'>
                            <div>{this._10S}</div>
                            <div>10</div>
                        </div>
                        <div className='cardCol'>
                            <div>{this._QD}</div>
                            <div>3</div>
                        </div>
                        <div className='cardCol'>
                            <div>{this._8H}</div>
                            <div>0</div>
                        </div>
                        <div className='cardCol'>
                            <div>{this._9S}</div>
                            <div>0</div>
                        </div>
                        <div className='cardCol'>
                            <div>Trick</div>
                            <div>Points</div>
                            <div>24</div>
                        </div>
                    </div>
                </div>
            ),
        cards:
            (
                <div>
                    <h3>CARDS</h3>
                    <p>
                        Sheepshead uses a 52-card deck, but throws away the 2s, 3s, 4s, 5s, and 6s.
                        There is a trump suit, and three fail suits (clubs, spades, and hearts).
                    </p>
                    <div className="card-group">
                        <div>
                            {this._6C}{this._5S}{this._4H}{this._3D}{this._2C}
                        </div>
                        <div className="prohibit-group">
                            <img className='prohibit' src={'./img/prohibit.png'} alt='prohibit' />
                            <img className='prohibit' src={'./img/prohibit.png'} alt='prohibit' />
                            <img className='prohibit' src={'./img/prohibit.png'} alt='prohibit' />
                            <img className='prohibit' src={'./img/prohibit.png'} alt='prohibit' />
                            <img className='prohibit' src={'./img/prohibit.png'} alt='prohibit' />
                        </div>
                    </div>
                </div>
            ),
        suits:
            (
                <div>
                    <h3>SUITS</h3>
                    <p>
                        The trump suit includes all Queens, Jacks, and Diamonds, omitting cards we threw away.
                        The clubs suit includes all clubs except Queens, Jacks, and cards we threw away.
                        The other fail suits follow the same pattern as clubs.
                    </p>
                    <div className='cardRow'>
                        <p className='label'>
                            Trump
                        </p>
                        <div className='overlapCards'>
                            {this._QC}
                            {this._QS}
                            {this._QH}
                            {this._QD}
                            {this._JC}
                            {this._JS}
                            {this._JH}
                            {this._JD}
                        </div>
                        <div className='fullCards'>
                            {this._AD}
                            {this._10D}
                            {this._KD}
                            {this._9D}
                            {this._8D}
                            {this._7D}
                        </div>
                    </div>
                    <div className='cardRow'>
                        <p className='label'>
                            Fail
                        </p>
                        <div className='overlapCards'>
                        </div>
                        <div className='fullCards'>
                            {this._AC}
                            {this._10C}
                            {this._KC}
                            {this._9C}
                            {this._8C}
                            {this._7C}
                        </div>
                    </div>
                </div>
            ),
        tricks:
            (
                <div>
                    <h3>TRICKS</h3>
                    <p>
                        At the beginning of a trick, the starting player plays a card of a given suit.
                        Each following player must play a card of the same suit, if he or she has it.
                        Otherwise, following players can play any card they like.
                    </p>
                    <div className="row">
                        <div>
                            <div>{this._8S}</div>
                            <div>Starting suit is Spades</div>
                        </div>
                        <div>
                            <div>{this._10S}</div>
                            <div>followed with Spades</div>
                        </div>
                        <div>
                            <div>{this._QD}</div>
                            <div>had no Spades</div>
                        </div>
                        <div>
                            <div>{this._7C}</div>
                            <div>had no Spades</div>
                        </div>
                        <div>
                            <div>{this._QS}</div>
                            <div>had no Spades (Queens are Trump)</div>
                        </div>
                        <div>
                            <div>{this._AS}</div>
                            <div>followed with Spades</div>
                        </div>
                    </div>
                </div>
            ),
        winningTricks:
            (
                <div>
                    <h3>WINNING TRICKS</h3>
                    <p>
                        The trick winner is the person who played the most powerful card.
                        That total points in the trick depends on what cards were in the trick.
                    </p>
                    <div className="row">
                        <div>
                            <div>{this._8S}</div>
                        </div>
                        <div>
                            <div>{this._10S}</div>
                            <div>most powerful yet</div>
                        </div>
                        <div>
                            <div>{this._QD}</div>
                            <div>most powerful yet</div>
                        </div>
                        <div>
                            <div>{this._7C}</div>
                        </div>
                        <div>
                            <div>{this._QS}</div>
                            <div>Won trick</div>
                        </div>
                        <div>
                            <div>{this._AS}</div>
                        </div>
                    </div>
                </div>
            ),
        powerAndPoints:
            (
                <div>
                    <h3>POWER AND POINTS</h3>
                    <p>
                        This graph shows you relative power and exact points for each card.
                        The most powerful card in a trick is the most powerful trump, if any trump was played.
                        If all card played were fail cards, then the most power card from the suit that led determines the trick winner.
                        This chart will be available as you play the game as a 'cheat sheet'.
                    </p>
                    <CheatSheet />
                </div>
            ),
        deal:
            (
                <div>
                    <h3>DEAL</h3>
                    <p>
                        In a 5-player game, each player is dealt 6 cards.
                        In a 3-player game, each player is dealt 10 cards.
                        The two remaining cards are the blinds.
                        The picker gets the blinds.
                    </p>
                    <div className='dealtHands'>
                        <div className='overlapCards'>
                            {this._back}
                            {this._back}
                        </div>
                        <div className='overlapCards'>
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                        </div>
                        <div className='overlapCards'>
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                        </div>
                        <div className='overlapCards'>
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                        </div>
                        <div className='overlapCards'>
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                        </div>
                        <div className='overlapCards'>
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                            {this._back}
                        </div>
                    </div>
                </div>
            ),
        picking:
            (
                <div>
                    <h3>PICKING</h3>
                    <p>
                        Each player gets a chance to decide to be or not to be a picker.
                        Pickers are on the offense.
                        Pickers get to take the two blind cards into their hand.
                        The picker must then select two cards to bury, which may or may not include one or both blind cards.
                        Pickers who win a hand recieve more benefits then a defensive player who wins a hand.
                        Most pickers have several trump cards.
                    </p>
                    <div className="pickerCards">
                        <div className="stage">
                            <div>Picker's hand</div>
                            <div className='overlapCards'>
                                {this._QC}
                                {this._JD}
                                {this._AS}
                                {this._KH}
                                {this._QS}
                                {this._JS}
                            </div>
                            <div>Blinds</div>
                            <div className='overlapCards'>
                                {this._back}
                                {this._back}
                            </div>
                        </div>
                        <div className="arrow">➜</div>
                        <div className="stage">
                            <div>Picker's hand</div>
                            <div>with blinds</div>
                            <div className='overlapCards'>
                                {this._QC}
                                {this._JD}
                                {this._AS}
                                {this._KH}
                                {this._QS}
                                {this._JS}
                                {this._QH}
                                {this._AC}
                            </div>
                        </div>
                        <div className="arrow">➜</div>
                        <div className="stage">
                            <div>Picker's hand after</div>
                            <div>burying cards</div>
                            <div className='overlapCards'>
                                {this._QC}
                                {this._JD}
                                {this._KH}
                                {this._QS}
                                {this._JS}
                                {this._QH}
                            </div>
                            <div>Buried cards</div>
                            <div className='overlapCards'>
                                {this._back}
                                {this._back}
                            </div>
                        </div>
                    </div>
                </div>
            ),
        partners:
            (
                <div>
                    <h3>PARTNERS</h3>
                    <p>
                        There is more than one way to determine the partner.
                        We'll teach you the Jack-of-Diamonds method first.
                        The partner normally is the person with Jack-of-Diamonds in his or her hand.
                        If the picker has Jack-of-Diamonds in his or her hand (including blinds),
                        then the partner is the player holding the first card in this list not in the picker's hand.
                    </p>
                    <div className="diagram">
                        {this._JD}
                        {this._JH}
                        {this._JS}
                        {this._JC}
                        {this._QD}
                        {this._QH}
                        {this._QS}
                        {this._QC}
                    </div>
                </div>
            ),
        winningHand:
            (
                <div>
                    <h3>WINNING A HAND</h3>
                    <p>
                        There are a total of 120 points in every hand.
                        In order to win a hand, defenders must win at least 60 points.
                        For offense to win a hand, they must have at least 61 points, including points in the buried cards.
                    </p>
                    <p>
                        Below, offense and defense each won three tricks.
                        Offense has 56 points and defense has 64.
                    </p>
                    <div className="diagram">
                        <div className="player">
                            <h5>Picker and Partner</h5>
                            <div className="trick">
                                <div>{this._7S} 0</div>
                                <div>{this._KS} 4</div>
                                <div>{this._9D} 0</div>
                                <div>{this._10S} 10</div>
                                <div>{this._7C} 0</div>
                            </div>
                            <div className="trick">
                                <div>{this._8C} 0</div>
                                <div>{this._9C} 0</div>
                                <div>{this._10C} 10</div>
                                <div>{this._AD} 11</div>
                                <div>{this._8H} 0</div>
                            </div>
                            <div className="trick">
                                <div>{this._KC} 4</div>
                                <div>{this._8S} 0</div>
                                <div>{this._AC} 11</div>
                                <div>{this._JH} 3</div>
                                <div>{this._KH} 4</div>
                            </div>
                            <div className="trick">
                                <p>Buried</p>
                                <div>{this._7H} 0</div>
                                <div>{this._9H} 0</div>
                            </div>
                        </div>
                        <div className="divider" />
                        <div className="player">
                            <h5>Defense</h5>
                            <div className="trick">
                                <div>{this._KD} 4</div>
                                <div>{this._10D} 10</div>
                                <div>{this._QD} 3</div>
                                <div>{this._7D} 0</div>
                                <div>{this._JS} 2</div>
                            </div>
                            <div className="trick">
                                <div>{this._JD} 2</div>
                                <div>{this._10H} 10</div>
                                <div>{this._QH} 3</div>
                                <div>{this._8D} 0</div>
                                <div>{this._JC} 2</div>
                            </div>
                            <div className="trick">
                                <div>{this._QS} 3</div>
                                <div>{this._AH} 11</div>
                                <div>{this._QC} 3</div>
                                <div>{this._9S} 0</div>
                                <div>{this._AS} 11</div>
                            </div>
                        </div>
                    </div>
                </div>
            ),
        play:
            (
                <div>
                    <h3>That's enough for now</h3>
                    <p>
                        Go play a few hands, and come back to learn the game even better.
                    </p>
                    <p>
                        In your first game, we'll turn off a setting called "leasters".
                    </p>
                    <a href="/setup/create?leastersOn=false">Play!</a>
                </div>
            )
    };
    private _advancedSlides: { [slideName: string]: JSX.Element } = {
        coins:
            (
                <div>
                    <h3>COINS</h3>
                    <p>
                        At the end of a hand each player wins or looses coins based on points they won.
                        Coins accumulate over each hand, card points do not.
                    </p>
                </div>
            ),
        coinsPerPlayer:
            (
                <div>
                    <h3>COINS PER PLAYER</h3>
                    <p>
                        In each hand, coins won by the offensive side plus coins by the defensive side equals zero.
                        This chart shows coins won or lost based on the points held by defensive side.
                        The picker wins a different number of points depending on the number of offensive players and the existance of a partner.
                    </p>
                    <table>
                        <tbody>
                            <tr>
                                <td></td>
                                <td></td>
                                <td>3 player game</td>
                                <td colSpan={2}>5 player game w/ Partner</td>
                                <td>5 player game wo/ Partner</td>
                            </tr>
                            <tr>
                                <td>Defensive Points</td>
                                <td>Coins per defensive player</td>
                                <td>Coins for Picker</td>
                                <td>Coins for Picker</td>
                                <td>Coins for Partner</td>
                                <td>Coins for Picker</td>
                            </tr>
                            <tr>
                                <td>Won no tricks</td>
                                <td>-3</td>
                                <td>6</td>
                                <td>6</td>
                                <td>3</td>
                                <td>12</td>
                            </tr>
                            <tr>
                                <td>0 - 29</td>
                                <td>-2</td>
                                <td>4</td>
                                <td>4</td>
                                <td>2</td>
                                <td>8</td>
                            </tr>
                            <tr>
                                <td>30 - 59</td>
                                <td>-1</td>
                                <td>2</td>
                                <td>2</td>
                                <td>1</td>
                                <td>4</td>
                            </tr>
                            <tr>
                                <td>60 - 89</td>
                                <td>1</td>
                                <td>-2</td>
                                <td>-2</td>
                                <td>-1</td>
                                <td>-4</td>
                            </tr>
                            <tr>
                                <td>90 - 120</td>
                                <td>2</td>
                                <td>-4</td>
                                <td>-4</td>
                                <td>-2</td>
                                <td>-8</td>
                            </tr>
                            <tr>
                                <td>Won all tricks</td>
                                <td>3</td>
                                <td>-6</td>
                                <td>-9</td>
                                <td>0</td>
                                <td>-12</td>
                            </tr>
                        </tbody>
                    </table>
                </div>
            ),
        schneider:
            (
                <div>
                    <h3>Schneider</h3>
                    <p>
                        When the defensive players win at least 30 points, it is said that they got schneider.
                        When the offensive players win at least 31 points, that means that the offense got schneider.
                        When playing Sheepshead with physical cards, you might hear your teammates say "We lost, but at least they didn't get schneider."
                    </p>
                </div>
            ),
        goingAlone:
            (
                <div>
                    <h3>GOING IT ALONE</h3>
                    <p>
                        You can see now why someone would take the risk of picking
                        and why someone would take the risk of picking without accepting a partner, that is, "going it alone".
                        Both decisions give the picker the opportunity to gain more coins.
                    </p>
                </div>
            ),
        leasters:
            (
                <div>
                    <h3>LEASTERS</h3>
                    <p>
                        When no one picks, a leasters round can start.
                        In leasters, the player with the lowest number of points will win the round.
                        The leasters winner must win at least one trick.
                    </p>
                </div>
            ),
        leastersWithBlinds:
            (
                <div>
                    <h3>LEASTERS WITH BLINDS</h3>
                    <p>
                        In this edition of Sheepshead, blind cards are ignored in leasters.
                        In many groups, house rules may assign the blinds the the player that wins the first leasters trick or the last leasters trick.
                        In other groups, the dealer at the begining of the round, decides which trick will win the blinds along with the other cards in the trick.
                    </p>
                </div>
            ),
        calledAce:
            (
                <div>
                    <h3>CALLED-ACE</h3>
                    <p>
                        A second method to identify a partner is the called-ace.
                        The picker can choose the ace of a fail suit, and that card identifies the partner.
                        The picker must have a card of the same suit in his or her hand.
                        The picker must not lead a trick with his or her last card of the called suit.
                        The partner must not lead with the Ace of the called suit.
                    </p>
                </div>
            ),
        play2:
            (
                <div>
                    <h3>Play some more</h3>
                    <p>
                        Let's turn leasters on now. You can also experiment with the called-ace partner method if you like.
                    </p>
                    <a href="/setup/create?leastersOn=true"> Play! </a>
                </div>
            )
    };

    constructor(props: any) {
        super(props);
        this.state = {
            filmOffset: '0px',
            currentSlide: 0
        };
        this.onLinkClick = this.onLinkClick.bind(this);
        this.scroll = this.scroll.bind(this);
        window.setInterval(this.scroll, 33);
    }

    componentDidMount() {
        this.scrollToSlide(this.props.location.hash);
    }

    private scroll() {
        var doc = document.documentElement;
        var windowOffset = (window.pageYOffset || doc.scrollTop) - (doc.clientTop || 0);
        this.setState({ filmOffset: windowOffset + 'px' });

        var slideNo = windowOffset / window.innerHeight;
        slideNo = slideNo - slideNo % 1;
        this.setState({ currentSlide: slideNo });
    }

    private onLinkClick(e: any) {
        var parser = document.createElement('a');
        parser.href = e.target.href;
        this.scrollToSlide(parser.hash);
    }

    private scrollToSlide(hash: string) {
        var slideName = hash.substring(1);
        window.scroll(0, this._inputNodes[slideName].offsetTop);
    }

    private renderSlide(slideName: string, slideContent: JSX.Element) {
        var self = this;
        var getSlideElement = function (node: HTMLDivElement | null) { node !== null ? self._inputNodes[slideName] = node : 0 };
        var slideClass = 'slide ' + slideName;
        var slideKey = 'slide-' + slideName;
        return (
            <div className={slideClass} name={slideName} ref={node => getSlideElement(node)} key={slideClass}>
                <div className='content'>
                    {slideContent}
                </div>
            </div>
        );
    }

    private renderButton(slideName: string, buttonText: string, slideIndex: number) {
        var pressedClass = slideIndex == this.state.currentSlide ? 'pressed-button' : '';
        var className = 'jump-button ' + pressedClass;
        return (
            <a className={className} href={'directions#' + slideName} onClick={this.onLinkClick} key={'button-' + slideName}>{buttonText}</a>
        );
    }

    public render() {
        var buttonNumber = 1;

        var basicSlides = [];
        var basicButtons = [];
        for (var slideName in this._basicSlides) {
            var buttonText = slideName == 'play' ? 'Play' : buttonNumber.toString();
            basicButtons.push(this.renderButton(slideName, buttonText, buttonNumber-1));
            basicSlides.push(this.renderSlide(slideName, this._basicSlides[slideName]));
            ++buttonNumber;
        }

        var advancedSlides = [];
        var advancedButtons = [];
        for (var slideName in this._advancedSlides) {
            var buttonText = slideName == 'play2' ? 'Play' : buttonNumber.toString();
            advancedButtons.push(this.renderButton(slideName, buttonText, buttonNumber-1));
            advancedSlides.push(this.renderSlide(slideName, this._advancedSlides[slideName]));
            ++buttonNumber;
        }

        return (
            <div className='directions-film'>
                {basicSlides}
                {advancedSlides}
                <div className='button-group'>
                    <div className='button-row'>
                        {basicButtons}
                    </div>
                    <div className='button-row'>
                        {advancedButtons}
                    </div>
                </div>
            </div>
        );
    }
}