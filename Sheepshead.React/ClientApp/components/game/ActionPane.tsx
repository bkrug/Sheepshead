import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { IdUtils } from '../IdUtils';
import { FetchUtils } from '../FetchUtils';
import { render } from 'react-dom';
import DraggableCard from './DraggableCard';
import PickPane from './PickPane';
import BuryPane from './BuryPane';
import TrickPane from './TrickPane';
import HandSummaryPane from './HandSummaryPane';
import { PlayState, PickChoice } from './PlayState';

export interface ActionPaneState {
    gameId: string;
    playerId: string;
    turnType: string;
}

export interface ActionPaneProps extends React.Props<any> {
    gameId: string;
    onTrickEnd: () => void;
    onHandEnd: () => void;
}

export default class ActionPane extends React.Component<ActionPaneProps, ActionPaneState> {
    constructor(props: ActionPaneProps) {
        super(props);
        this.state = {
            gameId: props.gameId,
            playerId: IdUtils.getPlayerId(props.gameId) || '',
            turnType: '',
        };
        this.onPickComplete = this.onPickComplete.bind(this);
        this.onBuryComplete = this.onBuryComplete.bind(this);
        this.onTrickPhaseComplete = this.onTrickPhaseComplete.bind(this);
        this.onSummaryPhaseComplete = this.onSummaryPhaseComplete.bind(this);
        this.loadPlayState = this.loadPlayState.bind(this);
        this.loadPlayState();
    }

    private loadPlayState(): void {
        var self = this;
        FetchUtils.get(
            'Game/GetTurnType?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: string): void {
                self.setState({
                    turnType: json == 'BeginDeck' ? 'ReportHand' : json
                });
            });
    }

    private onPickComplete(): void {
        this.loadPlayState();
    }

    private onBuryComplete(): void {
        this.loadPlayState();
    }

    private onTrickPhaseComplete(): void {
        this.loadPlayState();
    }

    private onSummaryPhaseComplete(): void {
        this.props.onHandEnd();
        this.loadPlayState();
        var self = this;
        FetchUtils.post(
            'Game/StartDeck?gameId=' + this.state.gameId + '&playerId=' + this.state.playerId,
            function (json: PlayState): void {
                self.setState({
                    turnType: json.turnType == 'BeginDeck' ? 'ReportHand' : json.turnType
                });
            });
    }

    public selectRenderPhase() {
        switch (this.state.turnType) {
            case 'Pick':
                return (<PickPane
                    gameId={this.state.gameId}
                    onPick={this.onPickComplete} />);
            case 'Bury':
                return (<BuryPane
                    gameId={this.state.gameId}
                    onBury={this.onBuryComplete} />);
            case 'PlayTrick':
                return (<TrickPane
                    gameId={this.state.gameId}
                    onTrickEnd={this.props.onTrickEnd}
                    onTrickPhaseComplete={this.onTrickPhaseComplete}
                    trickCount={6} playerCount={5} />);
            case 'ReportHand':
                return (<HandSummaryPane
                    gameId={this.state.gameId}
                    onSummaryPhaseComplete={this.onSummaryPhaseComplete} />);
            case 'BeginDeck':
                return (<div>Begin Deck Phase</div>);
            default:
                return (
                    <div>
                        <h4>Other Phase</h4>
                        This is not a recognized phase.
                    </div>);
        }
    }

    public render() {
        return (
            <div>
                { this.selectRenderPhase() }
            </div>
        );
    }
}