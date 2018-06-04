import * as React from 'react';
import { RouteComponentProps } from 'react-router';
import { FetchUtils } from '../FetchUtils';
import DraggableCard from './DraggableCard';
import CardDropBox from './CardDropBox';
import { DragDropContext } from 'react-dnd'
import HTML5Backend, { NativeTypes } from 'react-dnd-html5-backend'
const update = require('immutability-helper')

export interface CardPaneState {
    filenumbers: string[],
    lastDroppedItem: string | null
}

export interface CardPaneProps {
    filenumbers: string;
}

@DragDropContext(HTML5Backend)
export default class CardPane extends React.Component<CardPaneProps, CardPaneState> {
    constructor(props: CardPaneProps) {
        super(props);
        this.state = {
            filenumbers: props.filenumbers.split(',') || [],
            lastDroppedItem: null
            //dustbins: [
            //    { accepts: [ItemTypes.GLASS], lastDroppedItem: null },
            //    { accepts: [ItemTypes.FOOD], lastDroppedItem: null },
            //    {
            //        accepts: [ItemTypes.PAPER, ItemTypes.GLASS, NativeTypes.URL],
            //        lastDroppedItem: null,
            //    },
            //    { accepts: [ItemTypes.PAPER, NativeTypes.FILE], lastDroppedItem: null },
            //],
            //boxes: [
            //    { name: 'Bottle', type: ItemTypes.GLASS },
            //    { name: 'Banana', type: ItemTypes.FOOD },
            //    { name: 'Magazine', type: ItemTypes.PAPER },
            //],
            //droppedBoxNames: [],
        }
    }

    public render() {
        return (
            <div>
                <CardDropBox accepts={['card']}
                    lastDroppedItem={null}
                    // tslint:disable-next-line jsx-no-lambda
                    onDrop={item => this.handleDrop(0, item)}
                />
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

    private handleDrop(index: number, item: { name: string }) {
        const { name } = item
        const droppedBoxNames = name ? { $push: [name] } : {}

        this.setState(
            {
                lastDroppedItem: name
            }
        );
    }
}