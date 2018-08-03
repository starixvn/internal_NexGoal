import React, { Component } from 'react';
import $ from 'jquery';
import 'jquery-ui/ui/widgets/sortable';

class Sortable extends Component {
    componentDidMount() {
        this.$node = $(this.refs.sortable);
        this.$node.sortable({
            revert: true,
            receive: (event, ui) => this.props.onReceive(event, ui),
            deactivate: (event, ui) => this.props.onDeactivate(event, ui),
            items: "tr:not(.unsortable), li:not(.unsortable)"
        });
    }

    componentWillUnmount() {
        this.$node.sortable('destroy');
    }

    componentWillReceiveProps(nextProps) {
        if (nextProps.enable !== this.props.enable) {
            this.$node.sortable(nextProps.enable ? 'enable' : 'disable');
        }
    }

    renderItems() {
        return this.props.data.map((item, i) =>
            <li key={i}>
                {item.nickName}
            </li>
        );
    }

    render() {
        return (
            <ul className="sortable" ref="sortable">
                {this.renderItems()}
            </ul>
        );
    }
}

export default Sortable;