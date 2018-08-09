import React, { Component } from 'react';
import $ from 'jquery';
import 'jquery-ui/ui/widgets/sortable';

class Sortable extends Component {
    componentDidMount() {
        this.$node = $(this.refs.sortable);
        this.$node.sortable({
            revert: true,
            start: function (event, ui) {
                ui.item.startPos = ui.item.index();
            },
            receive: (event, ui) => this.props.onReceive(event, ui),
            deactivate: (event, ui) => this.props.onDeactivate(event, ui),
            items: "tr:not(.unsortable), li:not(.unsortable)"
        });
    }

    componentWillUnmount() {
        this.$node.sortable('destroy');
    }

    componentDidUpdate(nextProps) {
        if (nextProps.enable !== this.props.enable) {
            this.$node.sortable(nextProps.enable ? 'enable' : 'disable');
        }
    }

    renderItems() {
        return this.props.data.map((item, i) =>
            <li key={i} className="alert alert-info">
                {this.props.enable &&
                    <a className="fa fa-times" />
                }
                <img src={item.base64Image} />
                <p>{`${item.nickName} (${item.fullName})`}</p>
            </li>
        );
    }

    render() {
        return (
            <ul id={this.props.id} className="sortable" ref="sortable">
                {this.renderItems()}
            </ul>
        );
    }
}

export default Sortable;