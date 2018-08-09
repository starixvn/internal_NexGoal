import React, { Component } from 'react';
import $ from 'jquery';
import 'jquery-ui/ui/widgets/draggable';

class Draggable extends Component {
    componentDidMount() {
        this.$node = $(this.refs.draggable);
        this.$node.draggable({
            connectToSortable: this.props.connectToSortable,
            helper: 'clone',
            revert: 'invalid'
        });
    }

    componentWillUnmount() {
        this.$node.draggable('destroy');
    }

    componentDidUpdate(nextProps) {
        if (nextProps.enable !== this.props.enable) {
            this.$node.sortable(nextProps.enable ? 'enable' : 'disable');
        }
    }

    render() {
        return (
            <li ref="draggable" id={this.props.id}>
                {this.props.label}
            </li>
        );
    }
}

export default Draggable;