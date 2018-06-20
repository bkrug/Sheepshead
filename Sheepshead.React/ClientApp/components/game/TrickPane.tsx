﻿import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import { PlayState, TrickChoice, CardSummary } from './PlayState';

export interface TrickPaneState {
    gameId: string;
    playerId: string;
    cardsPlayed: TrickChoice[][];
    legalMoves: (boolean | null)[],
    displayedCardsPlayed: TrickChoice[][];
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
}

export interface TrickPaneProps extends React.Props<any> {
    gameId: string;
    onTrickEnd: () => void;
    onTrickPhaseComplete: () => void;
    playerCount: number;
    trickCount: number;
}

export default class TrickPane extends React.Component<TrickPaneProps, TrickPaneState> {
    displayInterval: number;

    constructor(props: TrickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            cardsPlayed: [],
            legalMoves: [],
            displayedCardsPlayed: [[]],
            playerCards: [],
            requestingPlayerTurn: false,
        };
        this.trickChoice = this.trickChoice.bind(this);
        this.displayOneMorePlay = this.displayOneMorePlay.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
        this.displayInterval = setInterval(this.displayOneMorePlay, 500);
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                var trickCount = self.state.cardsPlayed.length;

                self.setState({
                    cardsPlayed: json.cardsPlayed,
                    requestingPlayerTurn: json.requestingPlayerTurn,
                    playerCards: json.playerCards,
                    legalMoves: json.playerCards.map((value: CardSummary, index: number) => {
                        return value.legalMove;
                    }),
                });
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && json.turnType == "PlayTrick";
            },
            1000);
    }

    private displayOneMorePlay(): void {
        var tricksToDisplay = this.state.displayedCardsPlayed.length;
        var playsToDisplay = this.state.displayedCardsPlayed[tricksToDisplay - 1].length + 1;
        if (playsToDisplay > this.state.cardsPlayed[tricksToDisplay - 1].length && tricksToDisplay < this.state.cardsPlayed.length) {
            ++tricksToDisplay;
            playsToDisplay = 0;
        }

        var tricks = this.state.cardsPlayed.slice(0, tricksToDisplay);
        tricks[tricksToDisplay - 1] = tricks[tricksToDisplay - 1].slice(0, playsToDisplay);
        this.setState({
            displayedCardsPlayed: tricks
        });

        if (this.state.displayedCardsPlayed[tricksToDisplay - 1].length >= this.props.playerCount) {
            this.props.onTrickEnd();
            if (this.state.displayedCardsPlayed.length >= this.props.trickCount) {
                this.props.onTrickPhaseComplete();
                clearInterval(this.displayInterval);
            }
        }
    }

    private trickChoice(card: DraggableCard): void {
        var self = this;
        FetchUtils.post(
            'Game/RecordTrickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&card=' + card.props.cardSummary.name,
            function (json: number[]): void {
                self.initializePlayStatePinging();
            }
        );
    }

    private renderOneTrick(playsInTrick: TrickChoice[]) {
        return (
            <div>
                {
                    Object.keys(playsInTrick).map((playerName, i) => (
                        <div key={i} style={{ display: "inline-block" }}>
                            <p>{playsInTrick[i].item1}</p>
                            <DraggableCard key={i} cardSummary={playsInTrick[i].item2} />
                        </div>
                    ))
                }
            </div>
        );
    }

    public render() {
        return (
            <div>
                <h4>Trick Phase</h4>
                <b>Trick {this.state.displayedCardsPlayed.length}</b>
                { this.renderOneTrick(this.state.displayedCardsPlayed[this.state.displayedCardsPlayed.length - 1]) }
                <div>
                    {
                        this.state.requestingPlayerTurn
                            ? <div><b>What card will you play?</b></div>
                            : <div></div>
                    }
                </div>
                {
                    this.state.playerCards.map((card: CardSummary, i: number) =>
                        <DraggableCard key={i} cardSummary={card} onClick={this.trickChoice} />
                    )
                }
            </div>
        );
    }
}