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

    componentWillReceiveProps(nextProps) {
        if (nextProps.enable !== this.props.enable) {
            this.$node.sortable(nextProps.enable ? 'enable' : 'disable');
        }
    }

    handleOnRemoveClick(event, evaluatorId) {
        this.props.onRemove(this.$node[0].id, evaluatorId);
    }

    renderItems() {
        return this.props.data.map((item, i) =>
            <li key={`evaluator${item.evaluatorId}`} className="alert alert-info">
                {this.props.enable &&
                    <a className="fa fa-times" onClick={(e) => this.handleOnRemoveClick(e, item.evaluatorId)} />
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