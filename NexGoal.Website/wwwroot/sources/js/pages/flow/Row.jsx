import React, { Component, Fragment } from 'react';

import commonFunctions from '../../common-functions';

class Row extends Component {
    constructor(props) {
        super(props);
    }

    renderCellAction() {
        const flowTemplate = this.props.flowTemplate;

        if (this.props.isHost) {
            return (
                <Fragment>
                    <a className="fa fa-pencil-square-o" title="Edit"
                        href={commonFunctions.getRootUrl() + 'Flow/Edit/' + flowTemplate.flowTemplateId}></a>
                    <a className="fa fa-trash-o remove" title="Delete"></a>
                </Fragment>
            );
        }

        return (
            <a className="fa fa-search" title="Details"
                href={commonFunctions.getRootUrl() + 'Flow/Details/' + flowTemplate.flowTemplateId}></a>
        );
    }

    render() {
        const flowTemplate = this.props.flowTemplate;

        return (
            <tr>
                <td className="text-center">{this.props.rowIndex}</td>
                <td>{flowTemplate.name}</td>
                <td>{flowTemplate.targetTypeName}</td>
                <td className="text-center"></td>
                <td>{flowTemplate.description}</td>
                <td className="text-center">{this.renderCellAction()}</td>
            </tr>
        );
    }
}

export default Row;