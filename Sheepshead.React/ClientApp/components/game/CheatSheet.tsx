import '../../css/game.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';

export interface CheatState {
    display: boolean
}

export interface CheatProps extends React.Props<any> {
}

export class CheatSheet extends React.Component<CheatProps, CheatState> {
    constructor(props: CheatProps) {
        super(props);
        this.state = {
            display: false
        };
        this.showModal = this.showModal.bind(this);
        this.hideModal = this.hideModal.bind(this);
    }

    private showModal(): void {
        this.setState({
            display: true
        });
    }

    private hideModal(): void {
        this.setState({
            display: false
        });
    }

    private renderModal() {
        return (
            <div className="modalDialog">
                <div>
                    <table>
                        <tr className='borderless'>
                            <td><b>Power</b></td>
                            <td colSpan={8} style={{ textAlign: 'left', verticalAlign: 'bottom' }}>←More</td>
                            <td colSpan={6} style={{ textAlign: 'right', verticalAlign: 'bottom' }}>Less→</td>
                        </tr>
                        <tr>
                            <td><h5>Trump</h5></td>
                            <td>Q♣</td>
                            <td>Q♠</td>
                            <td className='redCard'>Q♥</td>
                            <td className='redCard'>Q♦</td>
                            <td>J♣</td>
                            <td>J♠</td>
                            <td className='redCard'>J♥</td>
                            <td className='redCard'>J♦</td>
                            <td className='redCard'>A♦</td>
                            <td className='redCard'>10♦</td>
                            <td className='redCard'>K♦</td>
                            <td className='redCard'>9♦</td>
                            <td className='redCard'>8♦</td>
                            <td className='redCard'>7♦</td>
                        </tr>
                        <tr>
                            <td><h5>Club</h5></td>
                            <td colSpan={8}></td>
                            <td>A♣</td>
                            <td>10♣</td>
                            <td>K♣</td>
                            <td>9♣</td>
                            <td>8♣</td>
                            <td>7♣</td>
                        </tr>
                        <tr>
                            <td><h5>Spades</h5></td>
                            <td colSpan={8}></td>
                            <td>A♠</td>
                            <td>10♠</td>
                            <td>K♠</td>
                            <td>9♠</td>
                            <td>8♠</td>
                            <td>7♠</td>
                        </tr>
                        <tr>
                            <td><h5>Hearts</h5></td>
                            <td colSpan={8}></td>
                            <td className='redCard'>A♥</td>
                            <td className='redCard'>10♥</td>
                            <td className='redCard'>K♥</td>
                            <td className='redCard'>9♥</td>
                            <td className='redCard'>8♥</td>
                            <td className='redCard'>7♥</td>
                        </tr>
                        <tr>
                            <td><b>Points</b></td>
                            <td colSpan={4}>3</td>
                            <td colSpan={4}>2</td>
                            <td>11</td>
                            <td>10</td>
                            <td>4</td>
                            <td colSpan={3}>0</td>
                        </tr>
                    </table>
                    <h4>Power vs. Points</h4>
                    <p>The most powerful card is the one that determines the trick winner.</p>
                    <p>The trick winner gets the points of all of the cards played in that trick.</p>
                    <h4>Suit Power</h4>
                    <p>Trump cards are always more powerful than Fail cards (Clubs, Spades, Hearts).</p>
                    <p>Within a trick, the suit of the first card played is more powerful than a card of any other Fail suit.</p>
                    <p>Trump can also lead a suit.</p>
                </div>
            </div>
        );
    }

    public render() {
        return (
            <div>
                <a style={{ cursor: 'pointer' }} onMouseOver={this.showModal} onMouseOut={this.hideModal}>Show Cheat Sheet</a>
                { this.state.display ? this.renderModal() : <div></div> }
            </div>
        );
    }
}