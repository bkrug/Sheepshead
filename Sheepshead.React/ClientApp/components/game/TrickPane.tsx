import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import { PlayState, TrickChoice } from './PlayState';

export interface TrickPaneState {
    gameId: string;
    playerId: string;
    cardsPlayed: TrickChoice[];
    playerCards: string[];
    requestingPlayerTurn: boolean;
}

export interface TrickPaneProps extends React.Props<any> {
    gameId: string;
    //cardsPlayed: TrickChoice[];
    playerCards: string[];
    //requestingPlayerTurn: boolean;
}

export default class TrickPane extends React.Component<TrickPaneProps, TrickPaneState> {
    constructor(props: TrickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            cardsPlayed: [],
            playerCards: props.playerCards,
            requestingPlayerTurn: false
        };
        this.trickChoice = this.trickChoice.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    cardsPlayed: json.cardsPlayed,
                    requestingPlayerTurn: json.requestingPlayerTurn,
                    playerCards: json.playerCards
                });
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && json.turnType == "PlayTrick";
            },
            1000);
    }

    private trickChoice(card: DraggableCard): void {
        var self = this;
        FetchUtils.post(
            'Game/RecordTrickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&cardFilename=' + card.props.cardImgNo,
            function (json: number[]): void {
                self.initializePlayStatePinging();
            }
        );
    }

    public render() {
        return (
            <div>
                <h4>Trick Phase</h4>
                {
                    Object.keys(this.state.cardsPlayed).map((playerName, i) => (
                            <div key={i}>
                                <p>{this.state.cardsPlayed[i].item1}</p>
                                <DraggableCard key={i} cardImgNo={this.state.cardsPlayed[i].item2} />
                            </div>
                    ))
                }
                <div>
                    {
                        this.state.requestingPlayerTurn
                            ? <div>
                                <b>What card will you play?</b>
                            </div>
                            : <div></div>
                    }
                </div>
                {
                    this.state.playerCards.map((card: string, i: number) =>
                        <DraggableCard key={i} cardImgNo={card} onClick={this.trickChoice} />
                    )
                }
            </div>
        );
    }
}