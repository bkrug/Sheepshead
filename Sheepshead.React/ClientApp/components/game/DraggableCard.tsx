import * as React from 'react';
import * as PropTypes from 'prop-types';
import {
    ConnectDragSource,
    DragSource,
    DragSourceConnector,
    DragSourceMonitor,
} from 'react-dnd';

export interface CardProps {
    cardImgNo: string
    isDragging?: boolean
    connectDragSource?: ConnectDragSource
}

const boxSource = {
    beginDrag(props: CardProps) {
        return {
            name: props.cardImgNo,
        }
    },

    endDrag(props: CardProps, monitor: DragSourceMonitor) {
        const item = monitor.getItem()
        const dropResult = monitor.getDropResult()

        if (dropResult) {
            alert(`You dropped ${item.name} into ${dropResult.name}!`)
        }
    },
}

@DragSource(
    'card',
    boxSource,
    (connect: DragSourceConnector, monitor: DragSourceMonitor) => ({
        connectDragSource: connect.dragSource(),
        isDragging: monitor.isDragging(),
    }),
)
export default class DraggableCard extends React.Component<CardProps, any> {
    public static propTypes = {
        connectDragSource: PropTypes.func.isRequired,
        isDragging: PropTypes.bool.isRequired,
        card: PropTypes.string.isRequired,
    }

    public render() {
        const { cardImgNo, isDragging, connectDragSource } = this.props;
        const opacity = isDragging ? 0.2 : 1;
        if (connectDragSource)
            return (
                connectDragSource(<img src={'./img/' + cardImgNo + '.png'} alt={cardImgNo} style={{ opacity }} />)
            )
        else
            return (<div></div>)
	}
}