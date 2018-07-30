import '../../css/game.css';
import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { CheatSheet } from './CheatSheet';

export interface CheatState {
    display: boolean
}

export interface CheatProps extends React.Props<any> {
}

export class CheatSheetModal extends React.Component<CheatProps, CheatState> {
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
                    <CheatSheet />
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