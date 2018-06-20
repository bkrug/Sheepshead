import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import { PlayState, PickChoice, CardSummary } from './PlayState';

export interface PickPaneState {
    gameId: string;
    playerId: string;
    playerCards: CardSummary[];
    requestingPlayerTurn: boolean;
    buryCards: CardSummary[];
}

export interface PickPaneProps extends React.Props<any> {
    gameId: string;
    onBury: () => void;
}

export default class PickPane extends React.Component<PickPaneProps, PickPaneState> {
    private cardContainerStyle = { height: "96px" };

    constructor(props: PickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            playerCards: [],
            requestingPlayerTurn: false,
            buryCards: []
        };
        this.buryChoice = this.buryChoice.bind(this);
        this.recordBuryChoice = this.recordBuryChoice.bind(this);
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetBuryState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    playerCards: json.playerCards,
                    requestingPlayerTurn: json.requestingPlayerTurn
                });
                if (json.turnType != "Bury")
                    self.props.onBury();
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false && json.turnType == "Bury";
            },
            1000);
    }

    private buryChoice(card: DraggableCard): void {
        var buryList = this.state.buryCards;
        buryList.push(card.props.cardSummary);

        var heldList = this.state.playerCards;
        var index = heldList.indexOf(card.props.cardSummary);
        heldList.splice(index, 1);

        this.setState({
            buryCards: buryList,
            playerCards: heldList
        });
        
        if (this.state.buryCards.length >= 2)
            this.recordBuryChoice();
    }

    private recordBuryChoice(): void {
        var self = this;
        var url = 'Game/RecordBury?gameId=' + this.state.gameId
            + '&playerId=' + this.state.playerId
            + '&cards=' + this.state.buryCards[0].name
            + '&cards=' + this.state.buryCards[1].name;
        FetchUtils.post(url,
            function (json: number[]): void {
                self.props.onBury();
            }
        );
    }

    public render() {
        return (
            <div>
                {this.state.requestingPlayerTurn
                    ? <div>
                        <h4>Pick cards to bury</h4>
                        <b>Held Cards</b>
                        <div style={this.cardContainerStyle}>
                        {
                            this.state.playerCards.map((card: CardSummary, i: number) =>
                                <DraggableCard key={i} cardSummary={card} onClick={this.buryChoice} />
                            )
                        }
                        </div>
                        <b>Cards to Bury</b>
                        <div style={this.cardContainerStyle}>
                            {
                                this.state.buryCards.map((card: CardSummary, i: number) =>
                                    <DraggableCard key={i} cardSummary={card} />
                                )
                            }
                        </div>
                    </div>
                    : <div><h4>Bury Phase</h4>Waiting for Picker to bury cards.</div>
                }
            </div>
        );
    }
}