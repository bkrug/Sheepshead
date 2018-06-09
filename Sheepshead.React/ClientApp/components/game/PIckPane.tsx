import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';

export interface PickPaneState {
    gameId: string;
    playerId: string;
    pickChoices: any[];
    playerCards: string[];
    requestingPlayerTurn: boolean;
}

export interface PickPaneProps extends React.Props<any> {
    gameId: string;
    pickChoices: any[];
    playerCards: string[];
    requestingPlayerTurn: boolean;
}

export default class PickPane extends React.Component<PickPaneProps, PickPaneState> {
    constructor(props: PickPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            pickChoices: props.pickChoices,
            playerCards: props.playerCards,
            requestingPlayerTurn: props.requestingPlayerTurn
        };
        this.initializePlayStatePinging = this.initializePlayStatePinging.bind(this);
        this.initializePlayStatePinging();
    }

    private initializePlayStatePinging(): void {
        var self = this;
        FetchUtils.repeatGet(
            'Game/GetPlayState?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    pickChoices: json.pickChoices,
                    requestingPlayerTurn: json.requestingPlayerTurn
                });
                console.log(self.state.pickChoices[2].valueOf());
            },
            function (json: PlayState): boolean {
                return json.requestingPlayerTurn == false;
            },
            1000);
    }

    private pickChoice(willPick: boolean): void {
        console.log(willPick);
        var self = this;
        FetchUtils.post(
            'Game/RecordPickChoice?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId + '&willPick=' + willPick,
            function (json: number[]): void {
                //self.initializePlayStatePinging();
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
                    this.state.playerCards.map((card: string, i: number) =>
                        <DraggableCard key={i} cardImgNo={card} />
                    )
                }
            </div>
        );
    }
}