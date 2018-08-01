import React, { Component, Fragment } from 'react';

import commonFunctions from '../../common-functions';

class Row extends Component {
    constructor(props) {
        super(props);

        this.handleCheckboxChange = this.handleCheckboxChange.bind(this);
        this.handleDeleteClick = this.handleDeleteClick.bind(this);
    }

    handleDeleteClick() {
        this.props.handleDeleteClick(this.props.flowTemplate);
    }

    renderCellAction() {
        const flowTemplate = this.props.flowTemplate;

        if (this.props.isHost) {
            return (
                <Fragment>
                    <a className="fa fa-pencil-square-o" title="Edit"
                        href={commonFunctions.getRootUrl() + 'Flow/Edit/' + flowTemplate.flowTemplateId}></a>
                    <a className="fa fa-trash-o remove" title="Delete" onClick={this.handleDeleteClick}></a>
                </Fragment>
            );
        }

        return (
            <a className="fa fa-search" title="Details"
                href={commonFunctions.getRootUrl() + 'Flow/Details/' + flowTemplate.flowTemplateId}></a>
        );
    }

    handleCheckboxChange(event) {
    }

    render() {
        const flowTemplate = this.props.flowTemplate;
        const isHost = this.props.isHost;

        return (
            <tr>
                <td className="text-center">{this.props.rowIndex}</td>
                <td>{flowTemplate.name}</td>
                <td>{flowTemplate.targetTypeName}</td>
                <td className="text-center">
                    <input type='checkbox' checked={flowTemplate.isActive} disabled={!isHost} onChange={this.handleCheckboxChange} />
                </td>
                <td>{flowTemplate.description}</td>
                <td className="text-center">{this.renderCellAction()}</td>
            </tr>
        );
    }
}

export default Row;