import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import Card from './Card';
import { PlayState, TrickChoice, CardSummary } from './PlayState';

export interface TrickPaneState {
    gameId: string;
    playerId: string;
    cardsPlayed: TrickChoice[][];
    legalMoves: (boolean | null)[],
    displayedCardsPlayed: TrickChoice[][];
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
    currentTurn: string;
}

export interface TrickPaneProps extends React.Props<any> {
    gameId: string;
    onTrickEnd: () => void;
    onTrickPhaseComplete: () => void;
    playerCount: number;
    trickCount: number;
}

export default class TrickPane extends React.Component<TrickPaneProps, TrickPaneState> {
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
            currentTurn: ''
        };
        this.trickChoice = this.trickChoice.bind(this);
        this.displayOneMorePlay = this.displayOneMorePlay.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
        setTimeout(this.displayOneMorePlay, 500);
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
                    currentTurn: json.currentTurn,
                    legalMoves: json.playerCards.map((value: CardSummary, index: number) => {
                        return value.legalMove;
                    })
                });
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && json.turnType == "PlayTrick";
            },
            1000);
    }

    private displayOneMorePlay(): void {
        var tricksToDisplay = this.state.displayedCardsPlayed.length;
        var playsToDisplay = tricksToDisplay == 0 ? 1 : this.state.displayedCardsPlayed[tricksToDisplay - 1].length + 1;
        var allPlaysNowDisplayed = tricksToDisplay == 0 || this.state.cardsPlayed.length == 0 || playsToDisplay > this.state.cardsPlayed[tricksToDisplay - 1].length;
        var notAllTricksDisplayed = tricksToDisplay < this.state.cardsPlayed.length;
        if (allPlaysNowDisplayed && notAllTricksDisplayed) {
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
            if (this.state.displayedCardsPlayed.length >= this.props.trickCount)
                setTimeout(this.props.onTrickPhaseComplete, 2000);
            else
                setTimeout(this.displayOneMorePlay, 2000);
        }
        else
            setTimeout(this.displayOneMorePlay, 500);
    }

    private trickChoice(card: Card): void {
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
                            <Card key={i} cardSummary={playsInTrick[i].item2} />
                        </div>
                    ))
                }
            </div>
        );
    }

    public render() {
        var allCardsDisplayed =
            this.state.displayedCardsPlayed.length == this.state.cardsPlayed.length
            && this.state.displayedCardsPlayed.slice(-1)[0].length == this.state.cardsPlayed.slice(-1)[0].length;
        var waitingForAnotherPlayer =            
            this.state.currentTurn
            && !this.state.requestingPlayerTurn;

        return (
            <div>
                <h4>Trick Phase</h4>
                <b>Trick {this.state.displayedCardsPlayed.length}</b>
                {this.renderOneTrick(this.state.displayedCardsPlayed[this.state.displayedCardsPlayed.length - 1])}
                <div>
                    {
                        allCardsDisplayed && waitingForAnotherPlayer
                            ? 'Waiting for ' + this.state.currentTurn + ' to take his or her turn.'
                            : ''
                    }
                </div>
                <div>
                    {
                        allCardsDisplayed && this.state.requestingPlayerTurn
                            ? <div><b>What card will you play?</b></div>
                            : <div></div>
                    }
                </div>
                {
                    this.state.playerCards.map((card: CardSummary, i: number) =>
                        <Card key={i} cardSummary={card} onClick={this.trickChoice} />
                    )
                }
            </div>
        );
    }
}