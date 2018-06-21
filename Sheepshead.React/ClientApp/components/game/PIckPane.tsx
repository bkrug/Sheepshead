﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import { PlayState, PickChoice, CardSummary } from './PlayState';

export interface PickPaneState {
    gameId: string;
    playerId: string;
    pickChoices: PickChoice[];
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
}

export interface PickPaneProps extends React.Props<any> {
    gameId: string;
    onPick: () => void;
}

export default class PickPane extends React.Component<PickPaneProps, PickPaneState> {
    constructor(props: PickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            pickChoices: [],
            playerCards: [],
            requestingPlayerTurn: false
        };
        this.pickChoice = this.pickChoice.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPickState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    pickChoices: json.pickChoices,
                    requestingPlayerTurn: json.requestingPlayerTurn,
                    playerCards: json.playerCards
                });
                if (json.turnType == "Bury" || json.turnType == "PlayTrick")
                    self.props.onPick();
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && (json.turnType == "Pick" || json.turnType == "BeginDeck");
            },
            1000);
    }

    private pickChoice(willPick: boolean): void {
        var self = this;
        FetchUtils.post(
            'Game/RecordPickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&willPick=' + willPick,
            function (json: number[]): void {
                if (willPick)
                    self.props.onPick();
                else
                    self.initializePlayStatePinging();
            }
        );
    }

    public render() {
        return (
            <div>
                <h4>Pick Phase</h4>
                {
                    Object.keys(this.state.pickChoices).map((playerName, i) => (
                            <div key={i}>
                            <p>{this.state.pickChoices[i].item1 + (this.state.pickChoices[i].item2 ? ' picked.' : ' refused.')}</p>
                            </div>
                    ))
                }
                <div>
                    {
                        this.state.requestingPlayerTurn
                            ? <div>
                                <b>Do you want to pick?</b>
                                <button onClick={() => this.pickChoice(true)}>Yes</button>
                                <button onClick={() => this.pickChoice(false)}>No</button>
                            </div>
                            : <div></div>
                    }
                </div>
                {
                    this.state.playerCards.map((card: CardSummary, i: number) =>
                        <DraggableCard key={i} cardSummary={card} />
                    )
                }
            </div>
        );
    }
}