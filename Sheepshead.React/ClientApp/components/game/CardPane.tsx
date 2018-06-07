import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { FetchUtils } from '../FetchUtils';
import DraggableCard from './DraggableCard';
import CardDropBox from './CardDropBox';

export interface CardPaneState {
    filenumbers: string[]
}

export interface CardPaneProps {
    filenumbers: string
}

export default class CardPane extends React.Component<CardPaneProps, CardPaneState> {
    constructor(props: CardPaneProps) {
        super(props);
        this.state = {
            filenumbers: props.filenumbers.split(',') || []
        }
    }

    public render() {
        return (
            <div>
                <CardDropBox />
                <h4>These are your cards</h4>
                {
                    this.state && this.state.filenumbers
                        ? this.state.filenumbers.map((card: string, i: number) =>
                            <DraggableCard key={i} cardImgNo={card} />
                        )
                        : (<div />)
                }
            </div>
        );
    }
}